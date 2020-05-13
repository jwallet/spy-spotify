using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using EspionSpotify.Enums;
using EspionSpotify.Extensions;
using EspionSpotify.Models;
using NAudio.Lame;
using NAudio.Wave;

namespace EspionSpotify
{
    public class Recorder : IRecorder
    {
        public const int MP3_MAX_NUMBER_CHANNELS = 2;
        public const int MP3_MAX_SAMPLE_RATE = 48000;

        public int CountSeconds { get; set; }
        public bool Running { get; set; }

        private readonly UserSettings _userSettings;
        private readonly Track _track;
        private readonly IFrmEspionSpotify _form;
        private OutputFile _currentOutputFile;
        private WasapiLoopbackCapture _waveIn;
        private Stream _fileWriter;
        private Stream _tempWaveWriter;
        private string _tempFile;
        private readonly FileManager _fileManager;
        private readonly IFileSystem _fileSystem;

        public Recorder() { }

        public Recorder(IFrmEspionSpotify form, UserSettings userSettings, Track track, IFileSystem fileSystem)
        {
            _form = form;
            _fileSystem = fileSystem;
            _track = track;
            _userSettings = userSettings;
            _fileManager = new FileManager(_userSettings, _track, fileSystem);
        }

        public async Task Run()
        {
            if (_userSettings.InternalOrderNumber > _userSettings.OrderNumberMax) return;

            Running = true;
            await Task.Delay(50);

            _currentOutputFile = _fileManager.GetOutputFile();
            _tempFile = _fileManager.GetTempFile();

            _waveIn = new WasapiLoopbackCapture(_userSettings.AudioSession.AudioMMDevicesManager.AudioEndPointDevice);
            _waveIn.DataAvailable += WaveIn_DataAvailable;
            _waveIn.RecordingStopped += WaveIn_RecordingStopped;

            try
            {
                _tempWaveWriter = new WaveFileWriter(_tempFile, _waveIn.WaveFormat);
                _fileWriter = GetFileWriter();
            }
            catch (Exception ex)
            {
                Running = false;
                _form.UpdateIconSpotify(true, false);
                _form.WriteIntoConsole(I18nKeys.LogUnknownException, ex.Message);
                Console.WriteLine(ex.Message);
                return;
            }

            _waveIn.StartRecording();
            _form.WriteIntoConsole(I18nKeys.LogRecording, _currentOutputFile.File);

            while (Running)
            {
                await Task.Delay(50);
            }

            _waveIn.StopRecording();
        }

        private async void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // TODO: add buffer handler from argument: issue #100
            if (_tempWaveWriter != null)
            {
                await _tempWaveWriter.WriteAsync(e.Buffer, 0, e.BytesRecorded);
            }
        }

        private async void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (_tempWaveWriter == null) return;

            _tempWaveWriter.Dispose();

            if (_fileWriter == null) return;

            try
            {
                await WriteTempWaveToMediaFile();
                try { _fileSystem.File.Delete(_tempFile); }
                catch { }
            }
            catch (Exception ex)
            {
                Running = false;
                _form.WriteIntoConsole(I18nKeys.LogUnknownException, ex.Message);
                Console.WriteLine(ex.Message);
                return;
            }
            finally
            {
                _waveIn.Dispose();
                _fileWriter.Dispose();
            }
            
            if (CountSeconds < _userSettings.MinimumRecordedLengthSeconds)
            {
                _form.WriteIntoConsole(I18nKeys.LogDeleting, _currentOutputFile.File, _userSettings.MinimumRecordedLengthSeconds);
                _fileManager.DeleteFile(_currentOutputFile.ToPendingFileString());
                return;
            }

            var length = TimeSpan.FromSeconds(CountSeconds).ToString(@"mm\:ss");
            _form.WriteIntoConsole(I18nKeys.LogRecorded, _track.ToString(), length);

            _fileManager.RenameFile(_currentOutputFile.ToPendingFileString(), _currentOutputFile.ToString());

            await UpdateOutputFileBasedOnMediaFormat();
        }

        private async Task WriteTempWaveToMediaFile()
        {
            var restrictions = _waveIn.WaveFormat.GetMP3RestrictionCode();
            using (var reader = new WaveFileReader(_tempFile))
            {
                reader.Position = 0;
                if (_userSettings.MediaFormat == MediaFormat.Mp3 && restrictions.Any())
                {
                    await WriteWaveProviderReducerToMP3FileWriter(GetMp3WaveProvider(reader));
                }
                else
                {
                    await reader.CopyToAsync(_fileWriter);
                }
            }
        }

        private async Task UpdateOutputFileBasedOnMediaFormat()
        {
            switch (_userSettings.MediaFormat)
            {
                case MediaFormat.Mp3:
                    var mapper = new MediaTags.MapperID3(
                        _currentOutputFile.ToString(),
                        _track,
                        _userSettings.OrderNumberInMediaTagEnabled,
                        _userSettings.OrderNumberAsTag);
                    await mapper.SaveMediaTags();
                    return;
                default:
                    return;
            }
        }

        public Stream GetFileWriter()
        {
            switch(_userSettings.MediaFormat)
            {
                case MediaFormat.Mp3:
                    var waveFormat = GetWaveFormatMP3SupportedBasedOnWaveInFormat();
                    return new LameMP3FileWriter(_currentOutputFile.ToPendingFileString(), waveFormat, _userSettings.Bitrate);
                case MediaFormat.Wav:
                    return new WaveFileWriter(_currentOutputFile.ToPendingFileString(), _waveIn.WaveFormat);
                default:
                    throw new Exception("Failed to get FileWriter");
            }
        }

        public static bool TestFileWriter(IFrmEspionSpotify form, UserSettings settings)
        {
            var waveIn = new WasapiLoopbackCapture(settings.AudioSession.AudioMMDevicesManager.AudioEndPointDevice);
            switch (settings.MediaFormat)
            {
                case MediaFormat.Mp3:
                    try
                    {
                        using (var writer = new LameMP3FileWriter(new MemoryStream(), waveIn.WaveFormat, settings.Bitrate)) return true;
                    }
                    catch (ArgumentException ex)
                    {
                        return LogLameMP3FileWriterArgumentException(form, ex, waveIn.WaveFormat);
                    }
                    catch (Exception ex)
                    {
                        LogLameMP3FileWriterException(form, ex);
                        return false;
                    }
                case MediaFormat.Wav:
                    try
                    {
                        using (var writer = new WaveFileWriter(new MemoryStream(), waveIn.WaveFormat)) return true;
                    }
                    catch (Exception ex)
                    {
                        form.UpdateIconSpotify(true, false);
                        form.WriteIntoConsole(I18nKeys.LogUnknownException, ex.Message);
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                default:
                    return false;
            }
        }

        private static bool LogLameMP3FileWriterArgumentException(IFrmEspionSpotify form, ArgumentException ex, WaveFormat waveFormat)
        {
            var restrictions = waveFormat.GetMP3RestrictionCode();
            if (restrictions.Any())
            {
                if (restrictions.Contains(WaveFormatMP3Restriction.Channel))
                {
                    form.WriteIntoConsole(I18nKeys.LogUnsupportedNumberChannels, waveFormat.Channels);
                }
                if (restrictions.Contains(WaveFormatMP3Restriction.SampleRate))
                {
                    form.WriteIntoConsole(I18nKeys.LogUnsupportedRate, waveFormat.SampleRate);
                }
                return true;
            }

            form.UpdateIconSpotify(true, false);
            form.WriteIntoConsole(I18nKeys.LogUnknownException, ex.Message);
            return false;
        }

        private static void LogLameMP3FileWriterException(IFrmEspionSpotify form, Exception ex)
        {
            if (ex.Message.Contains("Unable to load DLL"))
            {
                form.WriteIntoConsole(I18nKeys.LogMissingDlls);
            }
            else
            {
                form.WriteIntoConsole(I18nKeys.LogUnknownException, ex.Message);
            }

            form.UpdateIconSpotify(true, false);
            Console.WriteLine(ex.Message);
        }

        private WaveFormat GetWaveFormatMP3SupportedBasedOnWaveInFormat()
        {
            return WaveFormat.CreateIeeeFloatWaveFormat(
                        Math.Min(MP3_MAX_SAMPLE_RATE, _waveIn.WaveFormat.SampleRate),
                        Math.Min(MP3_MAX_NUMBER_CHANNELS, _waveIn.WaveFormat.Channels));
        }

        private IWaveProvider GetWaveProviderMP3ChannelReducer(IWaveProvider stream)
        {
            var waveProvider = new NAudio.Wave.MultiplexingWaveProvider(new IWaveProvider[] { stream }, MP3_MAX_NUMBER_CHANNELS);
            waveProvider.ConnectInputToOutput(0, 0);
            waveProvider.ConnectInputToOutput(1, 1);
            return waveProvider;
        }

        private IWaveProvider GetWaveProviderMP3SamplerReducer(IWaveProvider stream)
        {
            return new NAudio.Wave.MediaFoundationResampler(stream, MP3_MAX_SAMPLE_RATE);
        }

        private async Task WriteWaveProviderReducerToMP3FileWriter(IWaveProvider stream)
        {
            var mp3WaveFormat = GetWaveFormatMP3SupportedBasedOnWaveInFormat();
            byte[] data = new byte[mp3WaveFormat.Channels * mp3WaveFormat.SampleRate * _waveIn.WaveFormat.Channels];
            int bytesRead;
            while ((bytesRead = stream.Read(data, 0, data.Length)) > 0)
            {
                await _fileWriter.WriteAsync(data, 0, bytesRead);
            }
        }

        private IWaveProvider GetMp3WaveProvider(IWaveProvider stream)
        {
            var restrictions = _waveIn.WaveFormat.GetMP3RestrictionCode();
            if (restrictions.Contains(WaveFormatMP3Restriction.Channel))
            {
                stream = GetWaveProviderMP3ChannelReducer(stream);
            }
            if (restrictions.Contains(WaveFormatMP3Restriction.SampleRate))
            {
                stream = GetWaveProviderMP3SamplerReducer(stream);
            }
            return stream;
        }
    }
}
