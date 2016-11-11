using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using NAudio.Lame;
using NAudio.Wave;

namespace EspionSpotify
{
    class Recorder
    {
        public enum Format { mp3, wav };
        public int count = 0;
        public bool Running;
        private string path;
        private LAMEPreset bitrate;
        private int minTime;
        private string charSeparator;
        private bool strucDossiers;
        private string LastFullPath;
        private frmEspionSpotify spotifyRecorderForm;
        private Format format;
        private Song song;
        public WasapiLoopbackCapture waveIn;
        public Stream writer;

        public Recorder(frmEspionSpotify spotifyRecorderForm, string path, LAMEPreset bitrate, Format format, Song song, int minTime, bool strucDossiers, string charSeparator)
        {
            this.spotifyRecorderForm = spotifyRecorderForm;
            this.path = path;
            this.bitrate = bitrate;
            this.format = format;
            this.song = song;
            this.minTime = minTime;
            this.strucDossiers = strucDossiers;
            this.charSeparator = charSeparator;
        }

        public void Run()
        {
            Running = true;
            waveIn = new WasapiLoopbackCapture();
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.RecordingStopped += waveIn_RecordingStopped;
            writer = GetFileWriter(waveIn);

            waveIn.StartRecording();
            Thread.Sleep(400);
            spotifyRecorderForm.PrintStatusLine(String.Format("Enregistrement de: {0} - {1}", song.Artist, song.Title));
            while (Running)
            {
                Thread.Sleep(30);
            }
            waveIn.StopRecording();
            
            
        }

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (writer != null)
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
            }
        }

        private void waveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (writer != null)
            {
                writer.Dispose();
            }

            if (count < minTime)
            {                
                if(count!=-1)
                    spotifyRecorderForm.PrintStatusLine(String.Format("[!] Effacement de: {0} - {1} [<{2}s]", song.Artist, song.Title, minTime.ToString()));
                else
                    spotifyRecorderForm.PrintStatusLine(String.Format("[!] Effacement de: {0} - {1}", song.Artist, song.Title, minTime.ToString()));
                File.Delete(LastFullPath);
            }
        }

        public Stream GetFileWriter(WasapiLoopbackCapture waveIn)
        {
            Stream writer;
            string insertArtistDir = null;
            string ArtistDir = normalize.RemoveDiacritics(song.Artist.ToString());
            ArtistDir = Regex.Replace(ArtistDir, @"[^'0-9a-zA-Z_-]", charSeparator);
            if (strucDossiers)
            {
                insertArtistDir = "//" + ArtistDir;
                Directory.CreateDirectory(path + "//" + ArtistDir);
            }
            else
            {
                insertArtistDir = null;
            }
            switch (format)
            {
                case Format.mp3:
                    ID3TagData tag = new ID3TagData
                    {
                        Title = song.Title,
                        Artist = song.Artist
                    };
                    writer = new LameMP3FileWriter(
                        GetFullPath(path + insertArtistDir, song, "mp3"),
                        waveIn.WaveFormat,
                        bitrate,
                        tag);
                    return writer;

                case Format.wav:
                default:
                    writer = new WaveFileWriter(
                        GetFullPath(path + insertArtistDir, song, "wav"),
                        waveIn.WaveFormat
                        );
                    return writer;
            }
        }

        private string GetFullPath(string path, Song song, string ending)
        {
            string niceSongName;
            if (strucDossiers)
            {
                niceSongName = normalize.RemoveDiacritics(song.Title.ToString());
                niceSongName = Regex.Replace(niceSongName, @"[^'0-9a-zA-Z_-]", charSeparator);
            }
            else
            {
                niceSongName = normalize.RemoveDiacritics(song.ToString());
                niceSongName = Regex.Replace(niceSongName, @"[^'0-9a-zA-Z_-]", charSeparator);
            }
            string fullPath = String.Format("{0}\\{1}.{2}", path, niceSongName, ending);
            int i = 2;

            while (File.Exists(fullPath))
            {
                fullPath = String.Format("{0}\\{1}_{3}.{2}", path, niceSongName, ending, i);
                i++;
            }
            LastFullPath = fullPath;
            Console.WriteLine(fullPath);
            return fullPath;
        }
    }
}
