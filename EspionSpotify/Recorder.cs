using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using NAudio.Lame;
using NAudio.Wave;
using TagLib;
using File = System.IO.File;

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
        private readonly FrmEspionSpotify _form;
        private readonly Format _format;
        private readonly Song _song;
        private string _currentFile;
        public WasapiLoopbackCapture WaveIn;
        public Stream Writer;
        private const string ApiKey = "c117eb33c9d44d34734dfdcafa7a162d"; //01a049d30c4e17c1586707acf5d0fb17 82eb5ead8c6ece5c162b461615495b18
        private const string RegexCompare = "[^0-9a-zA-Z_'()$#&=+.@!%-]";

        public bool SongGotDeleted { get; }

        public Recorder() { }

        public Recorder(FrmEspionSpotify espionSpotifyForm, string path, LAMEPreset bitrate, Format format, 
            Song song, int minTime, bool strucDossiers, string charSeparator, bool bCdTrack, int compteur)
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
                _form.WriteIntoConsole(_form.Rm.GetString($"logWriterIsNull"));
                return;
            }

            Thread.Sleep(500);

            WaveIn.StartRecording();
            _form.WriteIntoConsole(string.Format(_form.Rm.GetString($"logRecording") ?? "{0}", GetFileName(_path, _song, _format, false)));

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
                if (_format == Format.Mp3) SetTagLibDataToMp3();
                return;
            }

            _form.WriteIntoConsole(Count != -1
                ? string.Format(_form.Rm.GetString($"logDeletingTooShort") ?? "{0}",
                    GetFileName(_path, _song, _format, false), _minTime)
                : string.Format(_form.Rm.GetString($"logDeleting") ?? "{0}",
                    GetFileName(_path, _song, _format, false)));

            File.Delete(_currentFile);
        }

        private void SetTagLibDataToMp3()
        {
            var mp3 = TagLib.File.Create(_currentFile);

            var numTrackAlbum = -1;
            var albumTitle = "";
            var style = "";

            // add known tags
            if (_bCdTrack) mp3.Tag.Track = (uint)_compteur;
            mp3.Tag.Title = _song.Title;
            mp3.Tag.AlbumArtists = new[] { _song.Artist };
            mp3.Tag.Performers = new[] { _song.Artist };
#pragma warning disable 618
            mp3.Tag.Artists = new[] { _song.Artist };
#pragma warning restore 618

            // call api to get other tags
            var api = new XmlDocument();
            var artist = PCLWebUtility.WebUtility.UrlEncode(_song.Artist);
            var title = PCLWebUtility.WebUtility.UrlEncode(_song.Title);

            try
            {
                api.Load(
                    $"http://ws.audioscrobbler.com/2.0/?method=track.getInfo&api_key={ApiKey}&artist={artist}&track={title}");
            }
            catch (WebException e)
            {
                Console.Write(e.Message);
            }

            var apiReturn = api.DocumentElement;

            if (apiReturn != null)
            {
                var xmlNumTrackAlbum = apiReturn.SelectNodes("/lfm/track/album/@position");
                if (xmlNumTrackAlbum != null && xmlNumTrackAlbum.Count != 0)
                    numTrackAlbum = Convert.ToInt32(xmlNumTrackAlbum[0].InnerXml);
                var xmlAlbumTitle = apiReturn.SelectNodes("/lfm/track/album/title");
                if (xmlAlbumTitle != null && xmlAlbumTitle.Count != 0)
                    albumTitle = xmlAlbumTitle[0].InnerXml;
                var xmlStyle = apiReturn.SelectNodes("/lfm/track/toptags/tag/name");
                if (xmlStyle != null && xmlStyle.Count != 0)
                    style = xmlStyle[0].InnerXml;
            }
            
            if (numTrackAlbum != -1) mp3.Tag.Track = (uint)numTrackAlbum;
            mp3.Tag.Album = albumTitle;
            mp3.Tag.Genres = new[] { style };

            mp3.Save();

            // try to get and download all available covers and save it
            if (apiReturn != null)
            {
                var extraLargePicture = new Picture();
                var largePicture = new Picture();
                var mediumPicture = new Picture();
                var smallPicture = new Picture();

                var xmlAlbumArt = apiReturn.SelectNodes("/lfm/track/album/image[@size='extralarge']");
                if (xmlAlbumArt != null && xmlAlbumArt.Count != 0)
                {
                    var x = GetAlbumCover(xmlAlbumArt[0].InnerXml);
                    if (x != null) extraLargePicture = x;
                }
                xmlAlbumArt = apiReturn.SelectNodes("/lfm/track/album/image[@size='large']");
                if (xmlAlbumArt != null && xmlAlbumArt.Count != 0)
                {
                    var x = GetAlbumCover(xmlAlbumArt[0].InnerXml);
                    if (x != null) largePicture = x;
                }
                xmlAlbumArt = apiReturn.SelectNodes("/lfm/track/album/image[@size='medium']");
                if (xmlAlbumArt != null && xmlAlbumArt.Count != 0)
                {
                    var x = GetAlbumCover(xmlAlbumArt[0].InnerXml);
                    if (x != null) mediumPicture = x;
                }
                xmlAlbumArt = apiReturn.SelectNodes("/lfm/track/album/image[@size='small']");
                if (xmlAlbumArt != null && xmlAlbumArt.Count != 0)
                {
                    var x = GetAlbumCover(xmlAlbumArt[0].InnerXml);
                    if (x != null) smallPicture = x;
                }

                mp3.Tag.Pictures = new IPicture[4] { extraLargePicture, largePicture, mediumPicture, smallPicture };
                mp3.Save();
            }

            mp3.Dispose();
        }

        private static Picture GetAlbumCover(string link)
        {
            if (link == "") return null;
            var request = WebRequest.Create(link);
            using (var response = request.GetResponse().GetResponseStream())
            {
                if (response == null) return null;
                using (var reader = new BinaryReader(response))
                {
                    using (var memory = new MemoryStream())
                    {
                        var buffer = reader.ReadBytes(4096);
                        while (buffer.Length > 0)
                        {
                            memory.Write(buffer, 0, buffer.Length);
                            buffer = reader.ReadBytes(4096);
                        }
                        return new Picture
                        {
                            Type = PictureType.FrontCover,
                            MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                            Description = "Cover",
                            Data = memory.ToArray()
                        };
                    }
                }
            }
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
                try
                {
                    _currentFile = GetFileName(_path + insertArtistDir, _song, Format.Mp3);
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
                _currentFile = GetFileName(_path + insertArtistDir, _song, Format.Wav);
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

        private string GetFileName(string path, Song song, Format format, bool includePath = true)
        {
            string niceSongName;
            var track = _compteur != -1 ? _compteur.ToString("000") + _charSeparator : null;
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

            while (File.Exists(filename))
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
