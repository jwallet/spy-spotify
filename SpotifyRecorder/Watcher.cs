using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NAudio.Lame;

namespace EspionSpotify
{
    class Watcher
    {
        public static bool Running;
        public static bool Ready = true;
        private string Spying;
        private string[] titleSeperators;
        private frmEspionSpotify spotifyRecorderForm;
        private string path;
        private LAMEPreset bitrate;
        private int minTime;
        private string charSeparator;
        private bool strucDossiers;
        public int count = 0;
        private VolumeWin Sound;
        private Recorder.Format format;
        private Dictionary<string, string> spyWhat;

        public Watcher(frmEspionSpotify spotifyRecorderForm, String path, LAMEPreset bitrate, Recorder.Format format, VolumeWin Sound, int minTime, bool strucDossiers, string charSeparator)
        {
            titleSeperators = new string[] { " - " };
            this.spotifyRecorderForm = spotifyRecorderForm;
            this.path = path;
            this.bitrate = bitrate;
            this.format = format;
            this.Sound = Sound;
            this.minTime = minTime;
            this.strucDossiers = strucDossiers;
            this.charSeparator = charSeparator;
            spyWhat = new Dictionary<string, string>
            {
                {"chrome"," - Spotify - Google Chrome"},
                {"firefox", " - Spotify - Mozilla Firefox"},
                {"iexplore", " - Spotify - Internet Explorer"}
            };
        }

        public void Run()
        {
            if (!Running)
            {
                Ready = false;
                spotifyRecorderForm.PrintStatusLine("Début de l\'espionnage.");
                Recorder recorder = null;
                Song lastSong = null;
                Song currentSong = null;
                Process process2spy;
                string title = null;
                string songPlaying = null;
                string lastTitle = null;
                bool bPause = false;
                bool bAttente = false;
                bool bUnmute = false;
                Running = true;
                process2spy = FindProcess();
                title = GetTitle(process2spy, songPlaying);
                if (title == null)//spotify not running -- cancel recording session
                {
                    spotifyRecorderForm.PrintStatusLine("[!] Veuillez démarrer l\'application Spotify ou Spotify Web.");
                    Running = false;
                }
                else
                {
                    if (Spying=="Spotify")
                    {
                        spotifyRecorderForm.PrintStatusLine("Application Spotify détectée.");
                    }
                    else
                    {
                        if (Spying != "iexplore")
                        {
                            spotifyRecorderForm.PrintStatusLine(String.Format("Spotify Web Player détecté sous {0}. Conserver cet onglet actif.", Spying));
                            if(Spying=="chrome")
                                spotifyRecorderForm.PrintStatusLine("[!] Astuce: Pour aucune publicité, ajouter uBlock Origin extension à Chrome.");
                        }
                        else
                            spotifyRecorderForm.PrintStatusLine("Spotify Web Player détecté sous internet explorer. Conserver cet onglet actif.");
                        if (Spying != "chrome")
                            spotifyRecorderForm.PrintStatusLine("[!] Utilisez Chrome ou l\'application Spotify pour une meilleure qualité audio.");
                    }
                    if (title != "Spotify") //spotify is playing a song, wait till next one plays -- first opt
                    {
                        spotifyRecorderForm.PrintStatusLine("En attente du prochain titre...");
                        bAttente = true;
                    }
                    else
                    {
                        if (title != songPlaying)
                        {
                            Sound = new VolumeWin();
                            Sound.SetToHigh(bUnmute, title, Spying); //mute others device. We are live!
                        }
                    }
                }
                while (Running)
                {
                    if (bAttente) //first opt -- wait next song before starting to record
                    {
                        while (bAttente && Running) //start recording when next song starts.
                        {
                            lastTitle = title;
                            title = GetTitle(process2spy, songPlaying);
                            if (title != lastTitle) bAttente = false;
                            Thread.Sleep(20);
                        }
                        Sound = new VolumeWin();
                        Sound.SetToHigh(bUnmute, title, Spying);//mute others device. We are live!
                    }
                    else//second opt OR normal behavior
                    {
                        if(currentSong!=null)
                            songPlaying = (currentSong.Title+" - "+currentSong.Artist).ToString();
                        title = GetTitle(process2spy, songPlaying);
                        currentSong = GetSong(title);
                        if (lastSong != null && currentSong == null && title != null)
                        {
                            if (title != "")
                            {
                                if(title!="Spotify")
                                {
                                    spotifyRecorderForm.PrintStatusLine(String.Format("Publicité: {0}", title));
                                }
                                //goes here if nothing or an ad plays. it means, we stop recording the current audio file.
                                lastSong = currentSong;
                                bPause = true;
                                if (recorder != null)
                                {
                                    recorder.count = count; //do we keep this last audio file, if too short ill delete it.
                                    recorder.Running = false;
                                }
                                count = 0;
                            }
                            else
                            {
                                if(Spying!="Spotify")
                                    spotifyRecorderForm.PrintStatusLine("[X] Connexion instable avec Spotify, mais l\'enregistrement continue.");
                                    Thread.Sleep(500);
                            }
                        }
                        
                        if (currentSong != null && !currentSong.Equals(lastSong))
                        {//goes here if the next song is detected, we close the current audio file and start a fresh one.
                            lastSong = currentSong;
                            if (bPause)
                                Thread.Sleep(100);
                            if (recorder != null)
                            {
                                recorder.count = count; //do we keep this last audio file, if too short ill delete it.
                                recorder.Running = false;
                            }
                            recorder = new Recorder(spotifyRecorderForm, path, bitrate, format, currentSong, minTime, strucDossiers, charSeparator);
                            Thread recorderThread = new Thread(recorder.Run);
                            recorderThread.Start();
                            count = 0;
                        }
                        //si spotify ferme ou crash
                        if (currentSong == null && title == null)
                        {
                            if (Spying == "Spotify")
                                spotifyRecorderForm.PrintStatusLine("[!] Spotify est fermé.");
                            else
                            {
                                spotifyRecorderForm.PrintStatusLine("[!] Connexion perdu avec Spotify Web. Télécharger leur application pour une meilleure stabilité.");
                            }
                            if (recorder != null)
                            {
                                recorder.count = -1; //do we keep this last audio file, if too short ill delete it.
                                recorder.Running = false;
                                recorder.waveIn.Dispose();
                                recorder.writer.Dispose();
                                process2spy.Dispose();
                            }
                            Running = false;
                            
                        }
                        Thread.Sleep(100);
                    }
                }
                //si le bouton stop est cliquer
                if (recorder != null)
                {
                    recorder.count = count;
                    recorder.Running = false;
                    recorder.waveIn.Dispose();
                    recorder.writer.Dispose();
                }
                spotifyRecorderForm.PrintStatusLine("Fin de l\'espionnage.");
                spotifyRecorderForm.PrintCurrentlyPlaying("");
                Ready = true;
                if(Spying!=null)
                    Sound.SetToHigh(!bUnmute, title, Spying);                
            }
        }

        private String GetTitle(Process process2spy, string songPlaying)
        {
            string Title;
            Process process = GetProcess(process2spy);
            if (process!=null)
            {
                if (Spying == "Spotify")
                {
                    Title = process.MainWindowTitle;
                    if (Title != "Spotify")
                        spotifyRecorderForm.PrintCurrentlyPlaying(Title);
                    else
                        spotifyRecorderForm.PrintCurrentlyPlaying("");
                }
                else
                {
                    if (process.MainWindowTitle != "")
                    {
                        Title = process.MainWindowTitle;
                        int i = Title.IndexOf(spyWhat[Spying]);
                        if (i != -1)
                            Title = Title.Remove(i);
                        i = Title.IndexOf(" Web Player");
                        if (i != -1)
                            Title = Title.Remove(i);

                        if (songPlaying != null)
                        {
                            if (Title.Equals(songPlaying))
                                Title = "Pause";
                        }
                    }
                    else
                        Title = process.MainWindowTitle;

                    if (!Title.Contains('▶'))
                    {
                        if (Title == "Pause")
                        {
                            spotifyRecorderForm.PrintCurrentlyPlaying("");
                            Title = "Spotify";
                        }
                        else if (songPlaying != null)
                            spotifyRecorderForm.PrintCurrentlyPlaying(Title);
                    }
                    else
                        spotifyRecorderForm.PrintCurrentlyPlaying(Title.Substring(2).ToString());
                }
                return Title;
            }
            return null;
        }

        private Song GetSong(String title)
        {
            if (title != null)
            {
                if(Spying!="Spotify")
                {
                    if(!title.Contains('▶'))
                    {
                        return null;
                    }
                    else
                    {
                        title = title.Remove(0, 2);
                    }
                }
                string[] tags = title.Split(titleSeperators, 2, StringSplitOptions.None);

                if (tags.Length == 2)
                {
                    if (Spying == "Spotify")
                        return new Song(tags[0], tags[1]);
                    else
                        return new Song(tags[1], tags[0]);
                }
            }
            return null;
        }

        private Process GetProcess(Process process2spy)
        {
            Process[] processlist = Process.GetProcesses();
            if (process2spy != null)
            {
                foreach (Process process in processlist)
                {
                    if (process.Id == process2spy.Id)
                        return process;
                }
            }
            return null;
        }

        private Process FindProcess()
        {
            Process[] processlist = Process.GetProcesses();
            
            foreach (Process process in processlist)
            {
                if (process.ProcessName.Equals("Spotify") && !String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    Spying = "Spotify";
                    return process;
                }
                else
                {
                    foreach (string app in spyWhat.Keys)
                    {
                        if (process.ProcessName.Equals(app) && !String.IsNullOrEmpty(process.MainWindowTitle) && process.MainWindowTitle.Contains(spyWhat[app]))
                        {
                            Spying = app;
                            return process;
                        }
                    }
                }
            }
            return null;
        }
    }
}
