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
    internal class Recorder: IRecorder
    {
        public int CountSeconds { get; set; }
        public bool Running { get; set; }

        private readonly UserSettings _userSettings;
        private readonly Track _track;
        private readonly IFrmEspionSpotify _form;
        private string _currentFile;
        private string _currentFilePending;
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

            _currentFile = _fileManager.BuildFileName(_userSettings.OutputPath);
            _currentFilePending = _fileManager.BuildSpytifyFileName(_currentFile);

            _writer = GetFileWriter(_currentFilePending, _waveIn, _userSettings);

            if (_writer == null)
            {
                Running = false;
                return;
            }

            _waveIn.StartRecording();
            _form.WriteIntoConsole("logRecording", _fileManager.GetFileName(_currentFile));

            while (Running)
            {
                await Task.Delay(50);
            }

            _waveIn.StopRecording();
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // TODO: add buffer handler from argument
            _writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        private void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (_writer != null)
            {
                _writer.Flush();
                _waveIn.Dispose();
                _writer.Dispose();
            }

            if (CountSeconds < _userSettings.MinimumRecordedLengthSeconds)
            {
                _form.WriteIntoConsole("logDeleting", _fileManager.GetFileName(_currentFile), _userSettings.MinimumRecordedLengthSeconds);
                _fileManager.DeleteFile(_currentFilePending);
                return;
            }

            var length = TimeSpan.FromSeconds(CountSeconds).ToString(@"mm\:ss");
            _form.WriteIntoConsole("logRecorded", _track.ToString(), length);

            _fileManager.Rename(_currentFilePending, _currentFile);

            if (!_userSettings.MediaFormat.Equals(MediaFormat.Mp3)) return;

            var mp3TagsInfo = new MediaTags.MP3Tags()
            {
                Track = _track,
                OrderNumberInMediaTagEnabled = _userSettings.OrderNumberInMediaTagEnabled,
                Count = _userSettings.OrderNumber,
                CurrentFile = _currentFile
            };

            Task.Run(async () => await mp3TagsInfo.SaveMediaTags());
        }

        private Stream GetFileWriter(string file, WasapiLoopbackCapture waveIn, UserSettings settings)
        {
            if (settings.MediaFormat.Equals(MediaFormat.Mp3))
            {
                try
                {
                    return new LameMP3FileWriter(file, waveIn.WaveFormat, settings.Bitrate);
                }
                catch (ArgumentException ex)
                {
                    var resource = "logUnknownException";
                    var args = ex.Message;

                    if (!Directory.Exists(settings.OutputPath))
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
                    return null;
                }
                catch (Exception ex)
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
                    return null;
                }
            }

            try
            {
                return new WaveFileWriter(file, waveIn.WaveFormat);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
