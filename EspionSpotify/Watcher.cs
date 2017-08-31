using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NAudio.CoreAudioApi;
using NAudio.Lame;

namespace EspionSpotify
{
    internal class Watcher
    {
        public static bool Running;
        public static bool Ready = true;
        public int CountSecs;

        private Recorder _recorder;
        private Song _lastSong;
        private Song _currentSong;

        private readonly FrmEspionSpotify _form;
        private readonly LAMEPreset _bitrate;
        private readonly Recorder.Format _format;
        private readonly Process _process2Spy;
        private readonly VolumeWin _sound;

        private readonly bool _bCdTrack;
        private readonly bool _strucDossiers;
        private readonly int _minTime;
        private readonly string _path;
        private readonly string _charSeparator;
        private readonly string[] _titleSeperators;

        private bool _bWait;
        private bool _bOnCommercialBreak;
        private bool _bSpotifyPlayIcon;
        private string _lastTitle;
        private string _title;
        private int _waitOneSec;

        private const bool Unmute = false;

        public int NumTrack { get; private set; }

        private bool NumTrackActivated => NumTrack != -1;
        private bool CommercialOrNothingPlays => _lastSong != null && _currentSong == null && _title != null;
        private bool NewSongIsPlaying => _currentSong != null && !_currentSong.Equals(_lastSong);
        private bool SpotifyClosedOrCrashed => _currentSong == null && _title == null;

        public Watcher(FrmEspionSpotify espionSpotifyForm, string path, LAMEPreset bitrate,
            Recorder.Format format, VolumeWin sound, int minTime, bool strucDossiers, 
            string charSeparator, bool bCdTrack, bool bNumFile, int cdNumTrack)
        {
            if (path == null) path = "";

            _titleSeperators = new [] {" - "};
            _form = espionSpotifyForm;
            _path = path;
            _bitrate = bitrate;
            _format = format;
            _sound = sound;
            NumTrack = bCdTrack || bNumFile ? cdNumTrack : -1;
            _minTime = minTime;
            _strucDossiers = strucDossiers;
            _charSeparator = charSeparator;
            _bCdTrack = bCdTrack;
            _process2Spy = FindProcess();
            _title = GetTitle(_process2Spy);
        }

        public void Run()
        {
            if (Running) return;

            Ready = false;
            Running = true;
            _waitOneSec = 0;

            _form.WriteIntoConsole(_form.Rm.GetString("logStarting"));

            SpotifyStatusBeforeSpying();

            while (Running)
            {
                if (_bWait)
                {
                    WaitUntilSpotifyStartPlaying();
                    _sound.SetToHigh(Unmute, _title);
                }
                else
                {
                    RecordSpotify();
                }
            }

            if (_recorder != null) DoIKeepLastSong(true);

            _form.WriteIntoConsole(_form.Rm.GetString("logStoping"));
            _form.UpdateStartButton();
            _form.UpdatePlayingTitle("Spotify");
            Ready = true;

            _sound.SetToHigh(!Unmute, _title);
        }

        private static bool SoundDetected()
        {
            var vol = new VolumeWin();
            return vol.DefaultAudioEndPointDevice.AudioMeterInformation.MasterPeakValue * 1000 > 0;
        }

        private void SpotifyStatusBeforeSpying()
        {
            if (_title == null)
            {
                _form.WriteIntoConsole(_form.Rm.GetString("logSpotifyNotRunning"));
                Running = false;
            }
            else
            {
                if (_title != "Spotify")
                {
                    _form.WriteIntoConsole(_form.Rm.GetString("logWaiting"));
                    _bWait = true;
                }
                else
                {
                    _sound.SetToHigh(Unmute, _title);
                }
            }
        }

        private void WaitUntilSpotifyStartPlaying()
        {
            while (_bWait && Running)
            {
                _lastTitle = _title;
                _title = GetTitle(_process2Spy);

                if (_title != _lastTitle) _bWait = false;

                Thread.Sleep(20);
            }
        }

        private void RecordSpotify()
        {
            _title = GetTitle(_process2Spy);
            _currentSong = GetSong(_title);

            if (CommercialOrNothingPlays)
            {
                _bOnCommercialBreak = true;
                _waitOneSec = 1;
                _lastSong = null;
                DoIKeepLastSong(true, true);

                if (_bSpotifyPlayIcon)
                {
                    _bSpotifyPlayIcon = false;
                    _form.UpdateIconSpotify(true);
                }
            }

            if (_bOnCommercialBreak && _waitOneSec == 10)
            {
                _waitOneSec = 0;
                _bOnCommercialBreak = false;

                if (SoundDetected())
                {
                    _form.WriteIntoConsole(string.Format(_form.Rm.GetString("logCommercialBreak"), _title));
                }
            }

            if (NewSongIsPlaying)
            {
                _lastSong = _currentSong;

                if (!_bSpotifyPlayIcon)
                {
                    _bSpotifyPlayIcon = true;
                    _form.UpdateIconSpotify();
                }

                if (_recorder != null) DoIKeepLastSong();

                UpdateNum();

                _recorder = new Recorder(_form, _path, _bitrate, _format, _currentSong, _minTime,
                    _strucDossiers, _charSeparator, _bCdTrack, NumTrack);

                var recorderThread = new Thread(_recorder.Run);
                recorderThread.Start();

                CountSecs = 0;
            }

            if (SpotifyClosedOrCrashed)
            {
                _form.WriteIntoConsole(_form.Rm.GetString("logSpotifyIsClosed"));

                if (_recorder != null)
                {
                    DoIKeepLastSong(true, false, true);
                    _process2Spy.Dispose();
                }

                Running = false;
            }

            if (_waitOneSec != 0) _waitOneSec++;

            Thread.Sleep(100);
        }

        private void DoIKeepLastSong(bool updateUi = false, bool thenReset = false, bool deleteItAnyway = false)
        {
            _recorder.Count = deleteItAnyway ? -1 : CountSecs;
            _recorder.Running = false;

            if (updateUi) UpdateNum();
            if (thenReset) CountSecs = 0;
        }

        private void UpdateNum()
        {
            if (!NumTrackActivated) return;

            if (CountSecs < _minTime) NumTrack--;

            NumTrack++;
            _form.UpdateNum(NumTrack);
        }

        private string GetTitle(Process process2Spy)
        {
            var process = GetProcess(process2Spy);

            if (process == null) return null;
            
            var title = process.MainWindowTitle;
            _form.UpdatePlayingTitle(title);

            return title;
        }

        private Song GetSong(string title)
        {
            var tags = title?.Split(_titleSeperators, 2, StringSplitOptions.None);
            return tags?.Length != 2 ? null : new Song(tags[0], tags[1]);
        }

        private static Process GetProcess(Process process2Spy)
        {
            var processlist = Process.GetProcesses();
            return process2Spy == null ? null : processlist.FirstOrDefault(process => process.Id == process2Spy.Id);
        }

        private static Process FindProcess()
        {
            var processlist = Process.GetProcesses();
            return processlist.FirstOrDefault(process => process.ProcessName.Equals("Spotify") && !string.IsNullOrEmpty(process.MainWindowTitle));
        }
    }
}
