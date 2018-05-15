using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using EspionSpotify.Properties;
using NAudio.Lame;
using NAudio.Wave;
using File = System.IO.File;

namespace EspionSpotify
{
    internal class Recorder
    {
        public enum Format { Mp3, Wav }
        public int Count = 0;
        public bool Running;
        private readonly string _path;
        private readonly LAMEPreset _bitrate;
        private readonly int _minTime;
        private readonly string _charSeparator;
        private readonly bool _strucDossiers;
        private readonly int _compteur;
        private readonly bool _bCdTrack;
        private readonly bool _bNumFile;
        private readonly FrmEspionSpotify _form;
        private readonly Format _format;
        private readonly Song _song;
        private string _currentFile;
        public WasapiLoopbackCapture WaveIn;
        public Stream Writer;
        private const int FirstSongNameCount = 1;

        private readonly string _windowsExlcudedChars = $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))}]";

        public bool SongGotDeleted { get; }

        public Recorder() { }

        public Recorder(FrmEspionSpotify espionSpotifyForm, string path, LAMEPreset bitrate, Format format, 
            Song song, int minTime, bool strucDossiers, string charSeparator, bool bCdTrack, bool bNumFile, int compteur)
        {
            SongGotDeleted = false;
            _form = espionSpotifyForm;
            _path = path;
            _bitrate = bitrate;
            _format = format;
            _song = song;
            _minTime = minTime;
            _strucDossiers = strucDossiers;
            _charSeparator = charSeparator;
            _compteur = compteur;
            _bCdTrack = bCdTrack;
            _bNumFile = bNumFile;
        }

        public void Run()
        {
            Running = true;
            WaveIn = new WasapiLoopbackCapture();

            WaveIn.DataAvailable += waveIn_DataAvailable;
            WaveIn.RecordingStopped += waveIn_RecordingStopped;

            Writer = GetFileWriter(WaveIn);

            if (Writer == null)
            {
                if (!Directory.Exists(_path))
                {
                    _form.WriteIntoConsole(FrmEspionSpotify.Rm.GetString($"logInvalidOutput"));
                    return;
                }
                _form.WriteIntoConsole(FrmEspionSpotify.Rm.GetString($"logWriterIsNull"));
                return;
            }

            WaveIn.StartRecording();
            _form.WriteIntoConsole(string.Format(FrmEspionSpotify.Rm.GetString($"logRecording") ?? "{0}", BuildFileName(_path, false)));

            while (Running)
            {
                Thread.Sleep(30);
            }
            WaveIn.StopRecording();
        }

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            Writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        private void waveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (Writer != null)
            {
                Writer.Flush();
                Writer.Dispose();
                WaveIn.Dispose();
            }

            if (Count >= _minTime)
            {
                if (_format != Format.Mp3) return;

                var mp3TagsInfo = new Mp3TagsInfo
                {
                    Song = _song,
                    BCdTrack = _bCdTrack,
                    Compteur = _compteur,
                    CurrentFile = _currentFile,
                    ClientId = Settings.Default.SpotifyClientId,
                    ClientSecret = Settings.Default.SpotifySecret
                };
                mp3TagsInfo.SetTagLibDataToMp3();

                return;
            }

            _form.WriteIntoConsole(string.Format(FrmEspionSpotify.Rm.GetString($"logDeletingTooShort") ?? $"{0}{1}", BuildFileName(_path, false), _minTime));

            File.Delete(_currentFile);
        }

        public Stream GetFileWriter(WasapiLoopbackCapture waveIn)
        {
            Stream writer;
            string insertArtistDir = null;
            var artistDir = Normalize.RemoveDiacritics(_song.Artist);
            artistDir = Regex.Replace(artistDir, _windowsExlcudedChars, string.Empty);

            if (_strucDossiers)
            {
                insertArtistDir = "//" + artistDir;
                Directory.CreateDirectory(_path + "//" + artistDir);
            }

            if (_format == Format.Mp3)
            {
                try
                {
                    _currentFile = BuildFileName(_path + insertArtistDir);
                    writer = new LameMP3FileWriter(
                        _currentFile,
                        waveIn.WaveFormat,
                        _bitrate);

                    return writer;
                }
                    catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }

            try
            {
                _currentFile = BuildFileName(_path + insertArtistDir);
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
            var ending = _format.ToString().ToLower();
            songName += count > FirstSongNameCount ? $"{_charSeparator}{count}" : string.Empty;
            return path != null ? $"{path}\\{songName}.{ending}" : $"{songName}.{ending}";
        }

        private string BuildFileName(string path, bool includePath = true)
        {
            string songName;
            var track = _compteur != -1 && _bNumFile ? $"{_compteur :000} " : null;

            if (_strucDossiers)
            {
                songName = Normalize.RemoveDiacritics(_song.Title);
                songName = Regex.Replace(songName, _windowsExlcudedChars, string.Empty);
            }
            else
            {
                songName = Normalize.RemoveDiacritics(_song.ToString());
                songName = Regex.Replace(songName, _windowsExlcudedChars, string.Empty);
            }

            var songNameTrackNumber = Regex.Replace($"{track}{songName}", "\\s", _charSeparator);
            var filename = GetFileName(songNameTrackNumber, FirstSongNameCount, includePath ? path : null);
            var count = FirstSongNameCount;

            while (File.Exists(GetFileName(songNameTrackNumber, count, path)))
            {
                if (includePath) count++;
                filename = GetFileName(songNameTrackNumber, count, includePath ? path : null);
                if (!includePath) count++;
            }

            return filename;
        }
    }
}
