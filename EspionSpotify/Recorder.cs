using EspionSpotify.AudioSessions;
using EspionSpotify.Enums;
using EspionSpotify.Extensions;
using EspionSpotify.Models;
using EspionSpotify.Native;
using NAudio.Lame;
using NAudio.Wave;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EspionSpotify
{
    public class Recorder : IRecorder, IDisposable
    {
        private bool _disposed = false;

        public const int MP3_MAX_NUMBER_CHANNELS = 2;
        public const int MP3_MAX_SAMPLE_RATE = 48000;
        public const long WAV_MAX_SIZE_BYTES = 4000000000;

        public int CountSeconds { get; set; }
        public bool Running { get; set; }

        private readonly UserSettings _userSettings;
        private readonly Track _track;
        private readonly IFrmEspionSpotify _form;
        private readonly IMainAudioSession _audioSession;
        private OutputFile _currentOutputFile;
        private WasapiLoopbackCapture _waveIn;
        private Stream _tempWaveWriter;
        private string _tempOriginalFile;
        private string _tempEncodeFile;
        private readonly FileManager _fileManager;
        private readonly IFileSystem _fileSystem;
        private bool _canBeSkippedValidated = false;
        private CancellationTokenSource _cancellationTokenSource;

        public bool IsSkipTrackActive
        {
            get => _userSettings.RecordRecordingsStatus == Enums.RecordRecordingsStatus.Skip
                && _fileManager.IsPathFileNameExists(_track, _userSettings, _fileSystem);
        }

        public Recorder() { }

        public Recorder(IFrmEspionSpotify form, IMainAudioSession audioSession, UserSettings userSettings, Track track, IFileSystem fileSystem)
        {
            _userSettings = new UserSettings();
            userSettings.CopyAllTo(_userSettings);

            _form = form;
            _audioSession = audioSession;
            _fileSystem = fileSystem;
            _track = track;
            _fileManager = new FileManager(_userSettings, _track, fileSystem);
        }

        public async Task Run(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;

            if (_userSettings.InternalOrderNumber > _userSettings.OrderNumberMax) return;

            Running = true;
            await Task.Delay(50);

            _tempOriginalFile = _fileManager.GetTempFile();

            _waveIn = new WasapiLoopbackCapture(_audioSession.AudioMMDevicesManager.AudioEndPointDevice);
            _waveIn.DataAvailable += WaveIn_DataAvailable;
            _waveIn.RecordingStopped += WaveIn_RecordingStopped;

            try
            {
                _tempWaveWriter = new WaveFileWriter(_tempOriginalFile, _waveIn.WaveFormat);
            }
            catch (Exception ex)
            {
                ForceStopRecording();
                _form.WriteIntoConsole(I18nKeys.LogUnknownException, ex.Message);
                Console.WriteLine(ex.Message);
                Program.ReportException(ex);
                return;
            }

            _waveIn.ShareMode = NAudio.CoreAudioApi.AudioClientShareMode.Shared;
            _waveIn.StartRecording();
            _form.WriteIntoConsole(I18nKeys.LogRecording, _track.ToString());

            while (Running)
            {
                if (_cancellationTokenSource.IsCancellationRequested) return;
                if (await StopRecordingIfTrackCanBeSkipped()) return;
                await Task.Delay(50);
            }

            _waveIn.StopRecording();
        }

        private async Task<bool> StopRecordingIfTrackCanBeSkipped()
        {
            if (_canBeSkippedValidated || !_track.MetaDataUpdated) return false;

            _canBeSkippedValidated = true;
            if (IsSkipTrackActive)
            {
                _form.WriteIntoConsole(I18nKeys.LogTrackExists, _track.ToString());
                await UpdateMediaTagsWhenSkippingTrack();
                ForceStopRecording();
                return true;
            }

            return false;
        }

        private async void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (_tempWaveWriter == null || !Running) return;

            if (_tempWaveWriter.Length < WAV_MAX_SIZE_BYTES / 2)
            {
                await _tempWaveWriter.WriteAsync(e.Buffer, 0, e.BytesRecorded);
            }
            else
            {
                _form.WriteIntoConsole(I18nKeys.LogRecordingDataExceeded, _track.ToString());
                ForceStopRecording();
            }
        }

        private async void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (_tempWaveWriter == null) return;

            await _tempWaveWriter.FlushAsync();
            var isTempWaveEmpty = _tempWaveWriter.Length == 0;

            _tempWaveWriter.Dispose();

            if (isTempWaveEmpty)
            {
                Running = false;
                _form.WriteIntoConsole(I18nKeys.LogSpotifyPlayingOutsideOfSelectedAudioEndPoint);
                _fileManager.DeleteFile(_tempOriginalFile);
                if (_waveIn != null) _waveIn.Dispose();
                return;
            }

            try
            {
                _tempEncodeFile = _fileManager.GetTempFile();
                await WriteWaveFileToMediaFile();
            }
            catch (Exception ex)
            {
                Running = false;
                _form.WriteIntoConsole(I18nKeys.LogUnknownException, ex.Message);
                Console.WriteLine(ex.Message);
                Program.ReportException(ex);
                return;
            }
            finally
            {
                if (_waveIn != null) _waveIn.Dispose();
                _fileManager.DeleteFile(_tempOriginalFile);
            }

            _currentOutputFile = _fileManager.GetOutputFile();

            if (CountSeconds < _userSettings.MinimumRecordedLengthSeconds)
            {
                _form.WriteIntoConsole(I18nKeys.LogDeleting, _currentOutputFile.ToString(), _userSettings.MinimumRecordedLengthSeconds);
                _fileManager.DeleteFile(_tempEncodeFile);
                return;
            }

            var length = TimeSpan.FromSeconds(CountSeconds).ToString(@"mm\:ss");
            _form.WriteIntoConsole(I18nKeys.LogRecorded, _currentOutputFile.ToString(), length);

            _fileManager.UpdateOutputFileWithLatestTrackInfo(_currentOutputFile, _track, _userSettings);
            _fileManager.RenameFile(_tempEncodeFile, _currentOutputFile.ToMediaFilePath());

            await UpdateMediaTagsFileBasedOnMediaFormat();

            _waveIn.DataAvailable -= WaveIn_DataAvailable;
            _waveIn.RecordingStopped -= WaveIn_RecordingStopped;

            Dispose();
        }

        private async Task WriteWaveFileToMediaFile()
        {
            if (_userSettings.MediaFormat == MediaFormat.Wav)
            {
                // copy instead of moving to be able to keep using common code
                _fileSystem.File.Copy(_tempOriginalFile, _tempEncodeFile);
            }
            else
            {
                await EncodeWaveFileToMediaFile();
            }
        }

        private async Task EncodeWaveFileToMediaFile()
        {
            var restrictions = _waveIn.WaveFormat.GetMP3RestrictionCode();
            using (var tempFileStream = _fileSystem.File.OpenRead(_tempOriginalFile))
            {
                tempFileStream.Position = 0;
                using (var tempReader = new WaveFileReader(tempFileStream))
                {
                    tempReader.Position = 0;
                    using (var mediaFileStream = _fileSystem.FileStream.Create(_tempEncodeFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        using (var mediaWriter = GetMediaFileWriter(mediaFileStream, _waveIn.WaveFormat))
                        {
                            if (_userSettings.MediaFormat == MediaFormat.Mp3 && restrictions.Any())
                            {
                                await WriteWaveProviderReducerToMP3FileWriter(mediaWriter, GetMp3WaveProvider(tempReader, _waveIn.WaveFormat));
                            }
                            else
                            {
                                await tempReader.CopyToAsync(mediaWriter, 81920, _cancellationTokenSource.Token);
                            }
                        }
                    }
                }
            }
        }

        private async Task UpdateMediaTagsWhenSkippingTrack()
        {
            if (!_userSettings.UpdateRecordingsID3TagsEnabled) return;
            
            _currentOutputFile = _fileManager.GetOutputFile();
            await UpdateMediaTagsFileBasedOnMediaFormat();
        }

        private async Task UpdateMediaTagsFileBasedOnMediaFormat()
        {
            if (!_fileSystem.File.Exists(_currentOutputFile.ToMediaFilePath())) return;

            switch (_userSettings.MediaFormat)
            {
                case MediaFormat.Mp3:
                    var mapper = new API.MapperID3(
                        _currentOutputFile.ToMediaFilePath(),
                        _track,
                        _userSettings);
                    await mapper.SaveMediaTags();
                    return;
                default:
                    return;
            }
        }

        public Stream GetMediaFileWriter(Stream stream, WaveFormat waveFormat)
        {
            switch (_userSettings.MediaFormat)
            {
                case MediaFormat.Mp3:
                    var supportedWaveFormat = GetWaveFormatMP3Supported(waveFormat);
                    return new LameMP3FileWriter(stream, supportedWaveFormat, _userSettings.Bitrate);
                case MediaFormat.Wav:
                    return new WaveFileWriter(stream, waveFormat);
                default:
                    throw new Exception("Failed to get FileWriter");
            }
        }

        public static bool TestFileWriter(IFrmEspionSpotify form, IMainAudioSession audioSession, UserSettings settings)
        {
            var waveIn = new WasapiLoopbackCapture(audioSession.AudioMMDevicesManager.AudioEndPointDevice);
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
                        Program.ReportException(ex);
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
                Program.ReportException(ex);
                form.WriteIntoConsole(I18nKeys.LogUnknownException, ex.Message);
            }

            form.UpdateIconSpotify(true, false);
            Console.WriteLine(ex.Message);
        }

        private WaveFormat GetWaveFormatMP3Supported(WaveFormat waveFormat)
        {
            return WaveFormat.CreateIeeeFloatWaveFormat(
                        Math.Min(MP3_MAX_SAMPLE_RATE, waveFormat.SampleRate),
                        Math.Min(MP3_MAX_NUMBER_CHANNELS, waveFormat.Channels));
        }

        private IWaveProvider GetWaveProviderMP3ChannelReducer(IWaveProvider stream)
        {
            var waveProvider = new MultiplexingWaveProvider(new IWaveProvider[] { stream }, MP3_MAX_NUMBER_CHANNELS);
            waveProvider.ConnectInputToOutput(0, 0);
            waveProvider.ConnectInputToOutput(1, 1);
            return waveProvider;
        }

        private IWaveProvider GetWaveProviderMP3SamplerReducer(IWaveProvider stream)
        {
            return new MediaFoundationResampler(stream, MP3_MAX_SAMPLE_RATE);
        }

        private async Task WriteWaveProviderReducerToMP3FileWriter(Stream mediaWriter, IWaveProvider stream)
        {
            var mp3WaveFormat = GetWaveFormatMP3Supported(_waveIn.WaveFormat);
            byte[] data = new byte[mp3WaveFormat.Channels * mp3WaveFormat.SampleRate * _waveIn.WaveFormat.Channels];
            int bytesRead;
            while ((bytesRead = stream.Read(data, 0, data.Length)) > 0)
            {
                await mediaWriter.WriteAsync(data, 0, bytesRead, _cancellationTokenSource.Token);
            }
        }

        private IWaveProvider GetMp3WaveProvider(IWaveProvider stream, WaveFormat waveFormat)
        {
            var restrictions = waveFormat.GetMP3RestrictionCode();
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

        private void ForceStopRecording()
        {
            _form.UpdateIconSpotify(true, false);
            Running = false;

            _waveIn.DataAvailable -= WaveIn_DataAvailable;
            _waveIn.RecordingStopped -= WaveIn_RecordingStopped;
            _waveIn.StopRecording();
            _waveIn.Dispose();

            _tempWaveWriter.Close();
            _tempWaveWriter.Dispose();

            _fileManager.DeleteFile(_tempOriginalFile);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_waveIn != null)
                {
                    _waveIn.DataAvailable -= WaveIn_DataAvailable;
                    _waveIn.RecordingStopped -= WaveIn_RecordingStopped;
                    _waveIn.StopRecording();
                    _waveIn.Dispose();
                }

                if (_tempWaveWriter != null)
                {
                    _tempWaveWriter.Close();
                    _tempWaveWriter.Dispose();
                }

                _fileManager.DeleteFile(_tempOriginalFile);

                if (_currentOutputFile != null)
                {
                    _fileManager.DeleteFile(_tempEncodeFile);
                }
            }

            _disposed = true;
        }
    }
}
