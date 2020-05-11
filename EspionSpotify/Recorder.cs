using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using EspionSpotify.Enums;
using EspionSpotify.Models;
using NAudio.Lame;
using NAudio.Wave;

namespace EspionSpotify
{
    public class Recorder : IRecorder
    {
        const int MP3_SUPPORTED_NUMBER_CHANNELS = 2;
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
            if (_userSettings.InternalOrderNumber > 999) return;

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
                await WriteStreamOutputToFileBasedOnNumberOfChannels();
            }
            catch (Exception ex)
            {
                Running = false;
                _form.UpdateIconSpotify(true, false);
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

        private async Task WriteStreamOutputToFileBasedOnNumberOfChannels()
        {
            using (var reader = new WaveFileReader(_tempFile))
            {
                reader.Position = 0;
                await reader.CopyToAsync(_fileWriter);
            }

            try { _fileSystem.File.Delete(_tempFile); }
            catch { }

            // TODO: #97 NAudio Multi channel pass through

            //if (_userSettings.MediaFormat == MediaFormat.Mp3 && _waveIn.WaveFormat.Channels > MP3_SUPPORTED_NUMBER_CHANNELS)
            //{
            //    //var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_waveIn.WaveFormat.SampleRate, MP3_SUPPORTED_NUMBER_CHANNELS);
            //    ////using (var converter = new WaveFormatConversionStream(waveFormat, reader))
            //    ////{
            //    ////    await converter.CopyToAsync(_fileWriter);
            //    ////}
            //    //using (var r = new RawSourceWaveStream(_streamOutput, waveFormat)) {
            //    //    await r.CopyToAsync(_fileWriter);
            //    //}
            //}
            //else
            //{
            //    using (var reader = new WaveFileReader(_streamOutput))
            //    {
            //        await reader.CopyToAsync(_fileWriter);
            //    }
            //}
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
                    var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_waveIn.WaveFormat.SampleRate, MP3_SUPPORTED_NUMBER_CHANNELS);
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
                        // TODO: #97 NAudio Multi channel pass through
                        //return true;
                        using (var writer = new LameMP3FileWriter(new MemoryStream(), waveIn.WaveFormat, settings.Bitrate)) return true;
                    }
                    catch (ArgumentException ex)
                    {
                        LogLameMP3FileWriterArgumentException(form, ex, settings.OutputPath);
                        return false;
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

        private static void LogLameMP3FileWriterArgumentException(IFrmEspionSpotify form, ArgumentException ex, string outputPath)
        {
            var resource = I18nKeys.LogUnknownException;
            var args = ex.Message;

            if (!Directory.Exists(outputPath))
            {
                resource = I18nKeys.LogInvalidOutput;
            }
            else if (ex.Message.StartsWith("Unsupported Sample Rate"))
            {
                resource = I18nKeys.LogUnsupportedRate;
            }
            else if (ex.Message.StartsWith("Access to the path"))
            {
                resource = I18nKeys.LogNoAccessOutput;
            }
            else if (ex.Message.StartsWith("Unsupported number of channels"))
            {
                var numberOfChannels = ex.Message.Length > 32 ? ex.Message.Remove(0, 31) : "?";
                var indexOfBreakLine = numberOfChannels.IndexOf("\r\n");
                numberOfChannels = numberOfChannels.Substring(0, indexOfBreakLine != -1 ? indexOfBreakLine : 0);
                resource = I18nKeys.LogUnsupportedNumberChannels;
                args = numberOfChannels;
            }

            form.UpdateIconSpotify(true, false);
            form.WriteIntoConsole(resource, args);
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
    }
}
