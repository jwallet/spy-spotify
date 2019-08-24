using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.Enums;
using EspionSpotify.Models;
using NAudio.Lame;
using NAudio.Wave;

namespace EspionSpotify
{
    internal class Recorder: IRecorder
    {
        private const long TICKS_PER_SECOND = 10000000;
        public int CountSeconds { get; set; }
        public bool Running { get; set; }

        private readonly UserSettings _userSettings;
        private readonly Track _track;
        private readonly IFrmEspionSpotify _form;
        private string _currentFile;
        private WasapiLoopbackCapture _waveIn;
        private Stream _writer;
        private FileManager _fileManager;

        public Recorder() { }

        public Recorder(IFrmEspionSpotify espionSpotifyForm, UserSettings userSettings, Track track)
        {
            _form = espionSpotifyForm;
            _track = track;
            _userSettings = userSettings;
            _fileManager = new FileManager(_userSettings, _track);
        }

        public void Run()
        {
            Running = true;
            Thread.Sleep(50);
            _waveIn = new WasapiLoopbackCapture(_userSettings.SpotifyAudioSession.AudioEndPointDevice);

            _waveIn.DataAvailable += WaveIn_DataAvailable;
            _waveIn.RecordingStopped += WaveIn_RecordingStopped;

            _writer = GetFileWriter(_waveIn);

            if (_writer == null)
            {
                return;
            }

            _waveIn.StartRecording();
            _form.WriteIntoConsole(string.Format(FrmEspionSpotify.Rm.GetString($"logRecording") ?? $"{0}", _fileManager.BuildFileName(_userSettings.OutputPath, false)));

            while (Running)
            {
                Thread.Sleep(50);
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
                _writer.Dispose();
                _waveIn.Dispose();
            }

            var timeSpan = new TimeSpan(TICKS_PER_SECOND * CountSeconds);
            var length = string.Format("{0}:{1:00}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
            _form.WriteIntoConsole(string.Format(FrmEspionSpotify.Rm.GetString($"logRecorded") ?? $"{0}{1}", _track.ToString(), length));

            if (CountSeconds >= _userSettings.MinimumRecordedLengthSeconds)
            {
                if (!_userSettings.MediaFormat.Equals(MediaFormat.Mp3)) return;

                var mp3TagsInfo = new MediaTags.MP3Tags()
                {
                    Track = _track,
                    OrderNumberInMediaTagEnabled = _userSettings.OrderNumberInMediaTagEnabled,
                    Count = _userSettings.OrderNumber,
                    CurrentFile = _currentFile
                };

                Task.Run(mp3TagsInfo.SaveMediaTags);

                return;
            }

            _form.WriteIntoConsole(string.Format(FrmEspionSpotify.Rm.GetString($"logDeletingTooShort") ?? $"{0}{1}", _fileManager.BuildFileName(_userSettings.OutputPath, false), _userSettings.MinimumRecordedLengthSeconds));

            _fileManager.DeleteFile(_currentFile);
        }

        private Stream GetFileWriter(WasapiLoopbackCapture waveIn)
        {
            Stream writer;
            string insertArtistDir = _fileManager.CreateDirectory();

            if (_userSettings.MediaFormat.Equals(MediaFormat.Mp3))
            {
                try
                {
                    _currentFile = _fileManager.BuildFileName(_userSettings.OutputPath + insertArtistDir);
                    writer = new LameMP3FileWriter(
                        _currentFile,
                        waveIn.WaveFormat,
                        _userSettings.Bitrate);

                    return writer;
                }
                catch (ArgumentException ex)
                {
                    var message = $"{FrmEspionSpotify.Rm.GetString($"logUnknownException")}: ${ex.Message}";

                    if (!Directory.Exists(_userSettings.OutputPath))
                    {
                        message = FrmEspionSpotify.Rm.GetString($"logInvalidOutput");
                    }
                    else if (ex.Message.StartsWith("Unsupported Sample Rate"))
                    {
                        message = FrmEspionSpotify.Rm.GetString($"logUnsupportedRate");
                    }
                    else if (ex.Message.StartsWith("Access to the path"))
                    {
                        message = FrmEspionSpotify.Rm.GetString("logNoAccessOutput");
                    }
                    else if (ex.Message.StartsWith("Unsupported number of channels"))
                    {
                        var numberOfChannels = ex.Message.Length > 32 ? ex.Message.Remove(0, 31) : "?";
                        var indexOfBreakLine = numberOfChannels.IndexOf("\r\n");
                        numberOfChannels = numberOfChannels.Substring(0, indexOfBreakLine != -1 ? indexOfBreakLine : 0);
                        message = String.Format(FrmEspionSpotify.Rm.GetString($"logUnsupportedNumberChannels"), numberOfChannels);
                    }

                    _form.WriteIntoConsole(message);
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }

            try
            {
                _currentFile = _fileManager.BuildFileName($"{_userSettings.OutputPath}{insertArtistDir}");
                writer = new WaveFileWriter(
                    _currentFile,
                    waveIn.WaveFormat
                );
                return writer;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
