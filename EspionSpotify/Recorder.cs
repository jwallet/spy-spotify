using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using NAudio.Lame;
using NAudio.Wave;

namespace EspionSpotify
{
    internal class Recorder
    {
        public enum Format { Mp3, Wav };
        public int Count = 0;
        public bool Running;
        private readonly string _path;
        private readonly LAMEPreset _bitrate;
        private readonly int _minTime;
        private readonly string _charSeparator;
        private readonly bool _strucDossiers;
        private readonly int _compteur;
        private readonly bool _bCdTrack;
        private readonly FrmEspionSpotify _espionSpotifyForm;
        private readonly Format _format;
        private readonly Song _song;
        public WasapiLoopbackCapture WaveIn;
        public Stream Writer;
        private const string Key = "c117eb33c9d44d34734dfdcafa7a162d";
        private const string RegexCompare = "[^0-9a-zA-Z_'()$#&=+.@!%-]";

        public bool SongGotDeleted { get; }

        public Recorder() { }

        public Recorder(FrmEspionSpotify espionSpotifyForm, string path, LAMEPreset bitrate, Format format, 
            Song song, int minTime, bool strucDossiers, string charSeparator, bool bCdTrack, int compteur)
        {
            SongGotDeleted = false;
            _espionSpotifyForm = espionSpotifyForm;
            _path = path;
            _bitrate = bitrate;
            _format = format;
            _song = song;
            _minTime = minTime;
            _strucDossiers = strucDossiers;
            _charSeparator = charSeparator;
            _compteur = compteur;
            _bCdTrack = bCdTrack;
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
                _espionSpotifyForm.PrintStatusLine(
                    "//Erreur lors de l'enregistrement: Format audio de votre ordinateur non supporté. Le format doit" +
                    " être '2 canaux, 24 bit, 48000 Hz (Studio Quality)' (Panneau de configuration > Son > Propriétés > Avancés).");
                return;
            }

            WaveIn.StartRecording();

            Thread.Sleep(400);
            _espionSpotifyForm.PrintStatusLine($"Enregistrement de: {GetFileName(_path, _song, _format, false)}");

            while (Running)
            {
                Thread.Sleep(30);
            }

            WaveIn.StopRecording();
        }

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            Writer?.Write(e.Buffer, 0, e.BytesRecorded);
        }

        private void waveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (Writer != null)
            {
                Writer.Flush();
                Writer.Dispose();
                Writer.Dispose();
                WaveIn.Dispose();
            }

            if (Count >= _minTime) return;

            _espionSpotifyForm.PrintStatusLine(Count != -1
                ? $"//Effacement de: {GetFileName(_path, _song, _format, false)} [<{_minTime}s]"
                : $"//Effacement de: {GetFileName(_path, _song, _format, false)}");

            File.Delete(GetFileName(_path, _song, _format, true, true));
        }

        public Stream GetFileWriter(WasapiLoopbackCapture waveIn)
        {
            Stream writer;
            string insertArtistDir = null;
            var artistDir = Normalize.RemoveDiacritics(_song.Artist);
            artistDir = Regex.Replace(artistDir, RegexCompare, _charSeparator);

            if (_strucDossiers)
            {
                insertArtistDir = "//" + artistDir;
                Directory.CreateDirectory(_path + "//" + artistDir);
            }

            if (_format == Format.Mp3)
            {
                var numTrackAlbum = "";
                var albumTitle = "";
                var style = "";

                var api = new XmlDocument();
                var artist = PCLWebUtility.WebUtility.UrlEncode(_song.Artist);
                var title = PCLWebUtility.WebUtility.UrlEncode(_song.Title);
                try { api.Load($"http://ws.audioscrobbler.com/2.0/?method=track.getInfo&api_key={Key}&artist={artist}&track={title}");}
                catch (WebException e) { Console.Write(e.Message); }
                var apiReturn = api.DocumentElement;

                if (apiReturn != null)
                {
                    var xmlNumTrackAlbum = apiReturn.SelectNodes("/lfm/track/album/@position");
                    if (xmlNumTrackAlbum != null && xmlNumTrackAlbum.Count != 0)
                        numTrackAlbum = xmlNumTrackAlbum[0].InnerXml;
                    var xmlAlbumTitle = apiReturn.SelectNodes("/lfm/track/album/title");
                    if (xmlAlbumTitle != null && xmlAlbumTitle.Count != 0)
                        albumTitle = xmlAlbumTitle[0].InnerXml;
                    var xmlStyle = apiReturn.SelectNodes("/lfm/track/toptags/tag/name");
                    if (xmlStyle != null && xmlStyle.Count != 0)
                        style = xmlStyle[0].InnerXml;
                }

                if (_bCdTrack) numTrackAlbum = $"{_compteur}";

                var tag = new ID3TagData
                {
                    Track = numTrackAlbum,
                    Title = _song.Title,
                    Artist = _song.Artist,
                    Album = albumTitle,
                    Genre = style
                };

                try
                {
                    writer = new LameMP3FileWriter(
                        GetFileName(_path + insertArtistDir, _song, Format.Mp3),
                        waveIn.WaveFormat,
                        _bitrate,
                        tag);
                    return writer;
                }
                    catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }

            writer = new WaveFileWriter(
                GetFileName(_path + insertArtistDir, _song, Format.Wav),
                waveIn.WaveFormat
            );

            return writer;
        }

        private string GetFileName(string path, Song song, Format format, bool includePath = true, bool tryingToDelete = false)
        {
            string niceSongName;
            var track = _compteur != -1 ? _compteur.ToString("00") + _charSeparator : null;
            var ending = format.ToString().ToLower();

            if (_strucDossiers)
            {
                niceSongName = Normalize.RemoveDiacritics(song.Title);
                niceSongName = Regex.Replace(niceSongName, RegexCompare, _charSeparator);
            }
            else
            {
                niceSongName = Normalize.RemoveDiacritics(song.ToString());
                niceSongName = Regex.Replace(niceSongName, RegexCompare, _charSeparator);
            }

            var filename = (includePath ? $"{path}\\{track + niceSongName}.{ending}" : $"{track + niceSongName}.{ending}");
            var i = 2;

            while (File.Exists(filename) && !tryingToDelete)
            {
                filename = includePath 
                    ? string.Format("{0}\\{1}{4}{3}.{2}", path, track + niceSongName, ending, i, _charSeparator)
                    : string.Format("{0}{3}{2}.{1}", track + niceSongName, ending, i, _charSeparator);
                i++;
            }

            return filename;
        }
    }
}
