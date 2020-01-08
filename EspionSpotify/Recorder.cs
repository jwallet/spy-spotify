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
    internal class Recorder : IRecorder
    {
        public int CountSeconds { get; set; }
        public bool Running { get; set; }

        private readonly UserSettings _userSettings;
        private readonly Track _track;
        private readonly IFrmEspionSpotify _form;
        private OutputFile _currentOutputFile;
        private WasapiLoopbackCapture _waveIn;
        private Stream _writer;
        private readonly FileManager _fileManager;
        private readonly IFileSystem _fileSystem;

        public Recorder() { }

        public Recorder(IFrmEspionSpotify espionSpotifyForm, UserSettings userSettings, Track track, IFileSystem fileSystem)
        {
            _form = espionSpotifyForm;
            _fileSystem = fileSystem;
            _track = track;
            _userSettings = userSettings;
            _fileManager = new FileManager(_userSettings, _track, fileSystem);
        }

        public async void Run()
        {
            Running = true;
            await Task.Delay(50);
            _waveIn = new WasapiLoopbackCapture(_userSettings.SpotifyAudioSession.AudioEndPointDevice);

            _waveIn.DataAvailable += WaveIn_DataAvailable;
            _waveIn.RecordingStopped += WaveIn_RecordingStopped;

            _currentOutputFile = _fileManager.GetOutputFile(_userSettings.OutputPath);

            _writer = new WaveFileWriter(_currentOutputFile.ToPendingFileString(), _waveIn.WaveFormat);

            if (_writer == null)
            {
                Running = false;
                return;
            }

            _waveIn.StartRecording();
            _form.WriteIntoConsole("logRecording", _currentOutputFile.File);

            while (Running)
            {
                await Task.Delay(50);
            }

            _waveIn.StopRecording();
        }

        private async void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // TODO: add buffer handler from argument
            if (_writer != null) await _writer.WriteAsync(e.Buffer, 0, e.BytesRecorded);
        }

        private async void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (_writer != null)
            {
                await _writer.FlushAsync();
                _writer.Dispose();
                _waveIn.Dispose();
            }

            if (CountSeconds < _userSettings.MinimumRecordedLengthSeconds)
            {
                _form.WriteIntoConsole("logDeleting", _currentOutputFile.File, _userSettings.MinimumRecordedLengthSeconds);
                _fileManager.DeleteFile(_currentOutputFile.ToPendingFileString());
                return;
            }

            var length = TimeSpan.FromSeconds(CountSeconds).ToString(@"mm\:ss");
            _form.WriteIntoConsole("logRecorded", _track.ToString(), length);

            await UpdateOutputFileBasedOnMediaFormat();    
        }

        private Stream GetFileWriter(string file, WaveFormat waveFormat, UserSettings settings)
        {
            switch (settings.MediaFormat)
            {
                case MediaFormat.Mp3:
                    try
                    {
                        return new LameMP3FileWriter(file, waveFormat, settings.Bitrate);
                    }
                    catch (ArgumentException ex)
                    {
                        LogLameMP3FileWriterArgumentException(ex, settings.OutputPath);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        LogLameMP3FileWriterException(ex);
                        return null;
                    }
                case MediaFormat.Wav:
                    try
                    {
                        return new WaveFileWriter(file, waveFormat);
                    }
                    catch (Exception ex)
                    {
                        _form.WriteIntoConsole("logUnknownException", ex.Message);
                        Console.WriteLine(ex.Message);
                        return null;
                    }
                default:
                    return null;
            }
        }

        private async Task UpdateOutputFileBasedOnMediaFormat()
        {
            switch (_userSettings.MediaFormat)
            {
                case MediaFormat.Wav:
                    _fileManager.Rename(_currentOutputFile.ToPendingFileString(), _currentOutputFile.ToString());
                    return;
                case MediaFormat.Mp3:
                    using (var reader = new WaveFileReader(_currentOutputFile.ToPendingFileString()))
                    {
                        using (var writer = GetFileWriter(_currentOutputFile.ToTranscodingToMP3String(), _waveIn.WaveFormat, _userSettings))
                        {
                            await reader.CopyToAsync(writer);
                            await writer.FlushAsync();
                        }
                    }

                    _fileManager.DeleteFile(_currentOutputFile.ToPendingFileString());
                    _fileManager.Rename(_currentOutputFile.ToTranscodingToMP3String(), _currentOutputFile.ToString());

                    var mp3TagsInfo = new MediaTags.MP3Tags()
                    {
                        Track = _track,
                        OrderNumberInMediaTagEnabled = _userSettings.OrderNumberInMediaTagEnabled,
                        Count = _userSettings.OrderNumber,
                        CurrentFile = _currentOutputFile.ToString()
                    };
                    await mp3TagsInfo.SaveMediaTags();

                    return;
                default:
                    return;
            }
        }

        private void LogLameMP3FileWriterArgumentException(ArgumentException ex, string outputPath)
        {
            var resource = "logUnknownException";
            var args = ex.Message;

            if (!Directory.Exists(outputPath))
            {
                resource = "logInvalidOutput";
            }
            else if (ex.Message.StartsWith("Unsupported Sample Rate"))
            {
                resource = "logUnsupportedRate";
            }
            else if (ex.Message.StartsWith("Access to the path"))
            {
                resource = "logNoAccessOutput";
            }
            else if (ex.Message.StartsWith("Unsupported number of channels"))
            {
                var numberOfChannels = ex.Message.Length > 32 ? ex.Message.Remove(0, 31) : "?";
                var indexOfBreakLine = numberOfChannels.IndexOf("\r\n");
                numberOfChannels = numberOfChannels.Substring(0, indexOfBreakLine != -1 ? indexOfBreakLine : 0);
                resource = "logUnsupportedNumberChannels";
                args = numberOfChannels;
            }

            _form.WriteIntoConsole(resource, args);
        }

        private void LogLameMP3FileWriterException(Exception ex)
        {
            if (ex.Message.Contains("Unable to load DLL"))
            {
                _form.WriteIntoConsole("logMissingDlls");
            }
            else
            {
                _form.WriteIntoConsole("logUnknownException", ex.Message);
            }

            Console.WriteLine(ex.Message);
        }
    }
}
