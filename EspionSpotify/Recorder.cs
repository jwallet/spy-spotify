using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.Enums;
using EspionSpotify.Models;
using NAudio.Lame;
using NAudio.Wave;
using File = System.IO.File;

namespace EspionSpotify
{
    internal class Recorder: IRecorder
    {
        public int CountSeconds { get; set; }
        public bool Running { get; set; }
        public bool SongGotDeleted { get; }

        private readonly UserSettings _userSettings;
        private readonly FrmEspionSpotify _form;
        private readonly Track _track;
        private string _currentFile;
        private WasapiLoopbackCapture _waveIn;
        private Stream _writer;
        private const int FirstSongNameCount = 1;

        private readonly string _windowsExlcudedChars = $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))}]";

        public Recorder() { }

        public Recorder(FrmEspionSpotify espionSpotifyForm, UserSettings userSettings, Track track)
        {
            SongGotDeleted = false;
            _form = espionSpotifyForm;
            _userSettings = userSettings;
            _track = track;
        }

        public void Run()
        {
            Running = true;
            Thread.Sleep(50);
            _waveIn = new WasapiLoopbackCapture();

            _waveIn.DataAvailable += WaveIn_DataAvailable;
            _waveIn.RecordingStopped += WaveIn_RecordingStopped;

            _writer = GetFileWriter(_waveIn);

            if (_writer == null)
            {
                return;
            }

            _waveIn.StartRecording();
            _form.WriteIntoConsole(string.Format(FrmEspionSpotify.Rm.GetString($"logRecording") ?? "{0}", BuildFileName(_userSettings.OutputPath, false)));

            while (Running)
            {
                Thread.Sleep(50);
            }

            _waveIn.StopRecording();
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
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

            _form.WriteIntoConsole(string.Format(FrmEspionSpotify.Rm.GetString($"logDeletingTooShort") ?? $"{0}{1}", BuildFileName(_userSettings.OutputPath, false), _userSettings.MinimumRecordedLengthSeconds));

            File.Delete(_currentFile);
        }

        private Stream GetFileWriter(WasapiLoopbackCapture waveIn)
        {
            Stream writer;
            string insertArtistDir = null;
            var artistDir = Normalize.RemoveDiacritics(_track.Artist);
            artistDir = Regex.Replace(artistDir, _windowsExlcudedChars, string.Empty);

            if (_userSettings.GroupByFoldersEnabled)
            {
                insertArtistDir = $"//{artistDir}";
                Directory.CreateDirectory($"{_userSettings.OutputPath}//{artistDir}");
            }

            if (_userSettings.MediaFormat.Equals(MediaFormat.Mp3))
            {
                try
                {
                    _currentFile = BuildFileName(_userSettings.OutputPath + insertArtistDir);
                    writer = new LameMP3FileWriter(
                        _currentFile,
                        waveIn.WaveFormat,
                        _userSettings.Bitrate);

                    return writer;
                }
                catch (Exception ex)
                {
                    var message = $"{FrmEspionSpotify.Rm.GetString($"logUnknownException")}: ${ex.Message}";

                    if (!Directory.Exists(_userSettings.OutputPath))
                    {
                        message = FrmEspionSpotify.Rm.GetString($"logInvalidOutput");
                    }
                    else if (ex.Message.StartsWith("Unsupported Sample Rate"))
                    {
                        message = FrmEspionSpotify.Rm.GetString($"logWriterIsNull");
                    }
                    else if (ex.Message.StartsWith("Access to the path"))
                    {
                        message = FrmEspionSpotify.Rm.GetString($"logNoAccessOutput");
                    }

                    _form.WriteIntoConsole(message);
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }

            try
            {
                _currentFile = BuildFileName($"{_userSettings.OutputPath}{insertArtistDir}");
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

        private string GetFileName(string songName, int count, string path = null)
        {
            var ending = _userSettings.MediaFormat.ToString().ToLower();
            songName += count > FirstSongNameCount ? $"{_userSettings.TrackTitleSeparator}{count}" : string.Empty;
            return path != null ? $"{path}\\{songName}.{ending}" : $"{songName}.{ending}";
        }

        private string BuildFileName(string path, bool includePath = true)
        {
            string songName;
            var track = _userSettings.OrderNumber != -1 && _userSettings.OrderNumberInfrontOfFileEnabled ? $"{_userSettings.OrderNumber:000} " : null;

            if (_userSettings.GroupByFoldersEnabled)
            {
                songName = Normalize.RemoveDiacritics(_track.Title);
                songName = Regex.Replace(songName, _windowsExlcudedChars, string.Empty);
            }
            else
            {
                songName = Normalize.RemoveDiacritics(_track.ToString());
                songName = Regex.Replace(songName, _windowsExlcudedChars, string.Empty);
            }

            var songNameTrackNumber = Regex.Replace($"{track}{songName}", "\\s", _userSettings.TrackTitleSeparator);
            var filename = GetFileName(songNameTrackNumber, FirstSongNameCount, includePath ? path : null);
            var count = FirstSongNameCount;

            while (_userSettings.DuplicateAlreadyRecordedTrack && File.Exists(GetFileName(songNameTrackNumber, count, path)))
            {
                if (includePath) count++;
                filename = GetFileName(songNameTrackNumber, count, includePath ? path : null);
                if (!includePath) count++;
            }

            return filename;
        }
    }
}
