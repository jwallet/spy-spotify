using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NAudio.Lame;

namespace EspionSpotify
{
    internal class Watcher
    {
        public static bool Running;
        public static bool Ready = true; 
        private string _spying;
        private readonly string[] _titleSeperators;
        private readonly FrmEspionSpotify _espionSpotifyForm;
        private readonly string _path;
        private readonly LAMEPreset _bitrate;
        private readonly int _minTime;
        private readonly string _charSeparator;
        private readonly bool _strucDossiers;
        public int CountSecs;
        private readonly bool _bCdTrack;
        private VolumeWin _sound;
        private readonly Recorder.Format _format;
        private readonly Dictionary<string, string> _spyWhat;

        private bool SoundIsPlaying => _sound.DefaultAudioEndPointDevice.AudioMeterInformation.MasterPeakValue > 0;

        public bool NumTrackActivated => NumTrack != -1;

        public int NumTrack { get; private set; }

        public Watcher(){}

        public Watcher(FrmEspionSpotify espionSpotifyForm, string path, LAMEPreset bitrate, Recorder.Format format, 
            VolumeWin sound, int minTime, bool strucDossiers, string charSeparator, bool bCdTrack, bool bNumFile, int cdNumTrack)
        {
            if (path == null) path = "";

            _titleSeperators = new [] { " - " };
            _espionSpotifyForm = espionSpotifyForm;
            _path = path;
            _bitrate = bitrate;
            _format = format;
            _sound = sound;
            NumTrack = bCdTrack || bNumFile ? cdNumTrack : -1;
            _minTime = minTime;
            _strucDossiers = strucDossiers;
            _charSeparator = charSeparator;
            _bCdTrack = bCdTrack;

            _spyWhat = new Dictionary<string, string>
            {
                {"chrome"," - Spotify - Google Chrome"},
                {"firefox", " - Spotify - Mozilla Firefox"},
                {"iexplore", " - Spotify - Internet Explorer"}
            };
        }

        public void Run()
        {
            if (Running) return;
            Ready = false;
            _espionSpotifyForm.PrintStatusLine("Début de l\'espionnage.");
            Recorder recorder = null;
            Song lastSong = null;
            Song currentSong = null;
            string songPlaying = null;
            var bPause = false;
            var bAttente = false;
            const bool bUnmute = false;
            Running = true;
            var process2Spy = FindProcess();
            var title = GetTitle(process2Spy, null);

            //spotify not running -- cancel recording session
            if (title == null)
            {
                _espionSpotifyForm.PrintStatusLine("[!] Veuillez démarrer l\'application Spotify ou Spotify Web.");
                Running = false;
            }
            else
            {
                if (_spying == "Spotify")
                {
                    _espionSpotifyForm.PrintStatusLine("Application Spotify détectée.");
                }
                else
                {
                    if (_spying != "iexplore")
                    {
                        _espionSpotifyForm.PrintStatusLine(
                            $"Spotify Web Player détecté sous {_spying}. Conserver cet onglet actif.");

                        if (_spying == "chrome")
                            _espionSpotifyForm.PrintStatusLine(
                                "[!] Astuce: Pour aucune publicité, ajouter uBlock Origin extension à Chrome.");
                    }
                    else
                    {
                        _espionSpotifyForm.PrintStatusLine("Spotify Web Player détecté sous internet explorer. Conserver cet onglet actif.");
                    }

                    if (_spying != "chrome") _espionSpotifyForm.PrintStatusLine("[!] Utilisez Chrome ou l\'application Spotify pour une meilleure qualité audio.");
                }

                //spotify is playing a song, wait till next one plays -- first opt
                if (title != "Spotify") 
                {
                    _espionSpotifyForm.PrintStatusLine("En attente du prochain titre...");
                    bAttente = true;
                }
                else
                {
                    _sound = new VolumeWin();
                    //mute others device. We are live!
                    _sound.SetToHigh(bUnmute, title, _spying); 
                }
            }
            while (Running)
            {
                //first opt -- wait next song before starting to record
                if (bAttente) 
                {
                    //start recording when next song starts.
                    while (bAttente && Running) 
                    {
                        var lastTitle = title;
                        title = GetTitle(process2Spy, null);

                        if (title != lastTitle) bAttente = false;

                        Thread.Sleep(20);
                    }
                    _sound = new VolumeWin();
                    //mute others device. We are live!
                    _sound.SetToHigh(bUnmute, title, _spying);
                }
                //second opt OR normal behavior
                else
                {
                    if(currentSong != null) songPlaying = currentSong.Title + " - " + currentSong.Artist;

                    title = GetTitle(process2Spy, songPlaying);
                    currentSong = GetSong(title);

                    if (lastSong != null && currentSong == null && title != null)
                    {
                        if (title != "")
                        {                            
                            if(title != "Spotify") _espionSpotifyForm.PrintStatusLine($"Publicité: {title}");
                            else if(SoundIsPlaying) _espionSpotifyForm.PrintStatusLine($"Publicité: Spotify");
                            else _espionSpotifyForm.PrintStatusLine("En attente du prochain titre...");

                            //goes here if nothing or an ad plays. it means, we stop recording the current audio file.
                            lastSong = null;
                            bPause = true;
                            //do we keep this last audio file, if too short ill delete it. count : length in seconds
                            recorder.Count = CountSecs;
                            recorder.Running = false;
                            UpdateNum();
                            CountSecs = 0;
                        }
                        else
                        {
                            if(_spying != "Spotify") _espionSpotifyForm.PrintStatusLine("[X] Connexion instable avec Spotify, mais l\'enregistrement continue.");

                            Thread.Sleep(500);
                        }
                    }
                        
                    if (currentSong != null && !currentSong.Equals(lastSong))
                    {
                        //goes here if the next song is detected, we close the current audio file and start a fresh one.
                        lastSong = currentSong;

                        if (bPause) Thread.Sleep(100);
                        
                        if (recorder != null)
                        {
                            //do we keep this last audio file, if too short ill delete it. count : length in seconds
                            recorder.Count = CountSecs; 
                            recorder.Running = false;
                        }

                        UpdateNum();

                        recorder = new Recorder(_espionSpotifyForm, _path, _bitrate, _format, currentSong, _minTime, 
                            _strucDossiers, _charSeparator, _bCdTrack, NumTrack);
                        var recorderThread = new Thread(recorder.Run);
                        recorderThread.Start();
                        CountSecs = 0;
                    }
                    
                    //if spotify crashes or closes
                    if (currentSong == null && title == null)
                    {
                        _espionSpotifyForm.PrintStatusLine(_spying == "Spotify"
                            ? "[!] Spotify est fermé."
                            : "[!] Connexion perdu avec Spotify Web. Télécharger leur application pour une meilleure stabilité.");

                        if (recorder != null)
                        {
                            //do we keep this last audio file, if too short ill delete it.
                            recorder.Count = -1; 
                            recorder.Running = false;

                            UpdateNum();

                            process2Spy.Dispose();
                        }

                        Running = false;
                    }
                    Thread.Sleep(100);
                }
            }
            
            //if button stop is clicked
            if (recorder != null)
            {
                recorder.Count = CountSecs;
                recorder.Running = false;

                UpdateNum();
            }

            _espionSpotifyForm.PrintStatusLine("Fin de l\'espionnage.");
            _espionSpotifyForm.PrintCurrentlyPlaying("");
            Ready = true;

            if (_spying!=null) _sound.SetToHigh(!bUnmute, title, _spying);
        }

        private void UpdateNum()
        {
            if (!NumTrackActivated) return;

            if (CountSecs < _minTime) NumTrack--;

            NumTrack++;
            _espionSpotifyForm.UpdateCdTrackNum(NumTrack);
        }

        private string GetTitle(Process process2Spy, string songPlaying)
        {
            var process = GetProcess(process2Spy);
            string title;

            if (process == null) return null;
            
            if (_spying == "Spotify")
            {
                title = process.MainWindowTitle;
                _espionSpotifyForm.PrintCurrentlyPlaying(title != "Spotify" ? title : "");
            }
            else
            {
                if (process.MainWindowTitle != "")
                {
                    title = process.MainWindowTitle;
                    var i = title.IndexOf(_spyWhat[_spying], StringComparison.Ordinal);

                    if (i != -1) title = title.Remove(i);

                    i = title.IndexOf(" Web Player", StringComparison.Ordinal);

                    if (i != -1) title = title.Remove(i);

                    if (songPlaying != null)
                    {
                        if (title.Equals(songPlaying)) title = "Pause";
                    }
                }
                else
                {
                    title = process.MainWindowTitle;
                }

                if (!title.Contains('▶'))
                {
                    if (title == "Pause")
                    {
                        _espionSpotifyForm.PrintCurrentlyPlaying("");
                        title = "Spotify";
                    }
                    else if (songPlaying != null)
                    {
                        _espionSpotifyForm.PrintCurrentlyPlaying(title);
                    }
                }
                else
                {
                    _espionSpotifyForm.PrintCurrentlyPlaying(title.Substring(2));
                }
            }
            return title;
        }

        private Song GetSong(string title)
        {
            if (title == null) return null;

            if (_spying!="Spotify")
            {
                if(!title.Contains('▶')) return null;

                title = title.Remove(0, 2);
            }

            var tags = title.Split(_titleSeperators, 2, StringSplitOptions.None);

            if (tags.Length != 2) return null;

            return _spying == "Spotify" ? new Song(tags[0], tags[1]) : new Song(tags[1], tags[0]);
        }

        private static Process GetProcess(Process process2Spy)
        {
            var processlist = Process.GetProcesses();
            return process2Spy == null ? null : processlist.FirstOrDefault(process => process.Id == process2Spy.Id);
        }

        private Process FindProcess()
        {
            var processlist = Process.GetProcesses();
            
            foreach (var process in processlist)
            {
                if (process.ProcessName.Equals("Spotify") && !string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    _spying = "Spotify";
                    return process;
                }

                foreach (var app in _spyWhat.Keys)
                {
                    if (!process.ProcessName.Equals(app) || string.IsNullOrEmpty(process.MainWindowTitle) ||
                        !process.MainWindowTitle.Contains(_spyWhat[app])) continue;

                    _spying = app;
                    return process;
                }

            }
            return null;
        }
    }
}
