using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using EspionSpotify.Properties;
using MetroFramework;
using MetroFramework.Forms;
using NAudio.Lame;
using EspionSpotify.Models;
using EspionSpotify.Enums;
using EspionSpotify.AudioSessions;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using EspionSpotify.Extensions;
using System.Linq;
using EspionSpotify.MediaTags;
using System.Text.RegularExpressions;

namespace EspionSpotify
{
    public sealed partial class FrmEspionSpotify : MetroForm, IFrmEspionSpotify
    {
        private IMainAudioSession _audioSession;
        private Watcher _watcher;
        private readonly UserSettings _userSettings;
        private readonly Analytics _analytics;
        private bool _toggleStopRecordingDelayed;

        public ResourceManager Rm { get; private set; }
        public static FrmEspionSpotify Instance { get; private set; }

        private string LogDate { get => $@"[{DateTime.Now:HH:mm:ss}] "; }

        public FrmEspionSpotify()
        {
            SuspendLayout();

            Instance = this;
            InitializeComponent();

            _userSettings = new UserSettings();
            BackImage = Resources.spytify_logo;

            if (string.IsNullOrEmpty(Settings.Default.Directory))
            {
                Settings.Default.Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                Settings.Default.Save();
            }

            if (string.IsNullOrEmpty(Settings.Default.AnalyticsCID))
            {
                Settings.Default.AnalyticsCID = Analytics.GenerateCID();
                Settings.Default.Save();
            }

            _analytics = new Analytics(Settings.Default.AnalyticsCID, Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Task.Run(async () => await _analytics.LogAction("launch"));


            var indexLanguage = Settings.Default.Language;
            var indexBitRate = Settings.Default.Bitrate;
            var indexAudioEndPointDevice = Settings.Default.AudioEndPointDeviceIndex.ToNullableInt();

            _userSettings.SpotifyAPIClientId = Settings.Default.SpotifyAPIClientId;
            _userSettings.SpotifyAPISecretId = Settings.Default.SpotifyAPISecretId;

            rbSpotifyAPI.Enabled = _userSettings.IsSpotifyAPISet;

            tcMenu.SelectedIndex = Settings.Default.TabNo;

            rbMp3.Checked = Settings.Default.MediaFormat == (int)MediaFormat.Mp3;
            rbWav.Checked = Settings.Default.MediaFormat == (int)MediaFormat.Wav;
            rbLastFMAPI.Checked = Settings.Default.MediaTagsAPI == (int)MediaTagsAPI.LastFM || !_userSettings.IsSpotifyAPISet;
            rbSpotifyAPI.Checked = Settings.Default.MediaTagsAPI == (int)MediaTagsAPI.Spotify && _userSettings.IsSpotifyAPISet;
            tbMinTime.Value = Settings.Default.MinimumRecordedLengthSeconds / 5;
            tgEndingSongDelay.Checked = Settings.Default.EndingSongDelayEnabled;
            tgAddSeparators.Checked = Settings.Default.TrackTitleSeparatorEnabled;
            tgNumTracks.Checked = Settings.Default.OrderNumberInMediaTagEnabled;
            tgNumFiles.Checked = Settings.Default.OrderNumberInfrontOfFileEnabled;
            tgAddFolders.Checked = Settings.Default.GroupByFoldersEnabled;
            txtPath.Text = Settings.Default.Directory;
            tgMuteAds.Checked = Settings.Default.MuteAdsEnabled;
            tgDuplicateAlreadyRecordedTrack.Checked = Settings.Default.DuplicateAlreadyRecordedTrack;
            tgRecordUnkownTrackType.Checked = Settings.Default.RecordUnknownTrackTypeEnabled;
            folderBrowserDialog.SelectedPath = Settings.Default.Directory;

            SetLanguageDropDown();

            var language = (LanguageType)indexLanguage;
            SetLanguage(language);

            cbBitRate.SelectedIndex = indexBitRate;
            cbLanguage.SelectedIndex = indexLanguage;

            _userSettings.AudioEndPointDeviceIndex = indexAudioEndPointDevice;
            _userSettings.Bitrate = ((KeyValuePair<LAMEPreset, string>)cbBitRate.SelectedItem).Key;
            _userSettings.DuplicateAlreadyRecordedTrack = Settings.Default.DuplicateAlreadyRecordedTrack;
            _userSettings.EndingTrackDelayEnabled = Settings.Default.EndingSongDelayEnabled;
            _userSettings.GroupByFoldersEnabled = Settings.Default.GroupByFoldersEnabled;
            _userSettings.MediaFormat = (MediaFormat)Settings.Default.MediaFormat;
            _userSettings.MinimumRecordedLengthSeconds = Settings.Default.MinimumRecordedLengthSeconds;
            _userSettings.OrderNumberInfrontOfFileEnabled = Settings.Default.OrderNumberInfrontOfFileEnabled;
            _userSettings.OrderNumberInMediaTagEnabled = Settings.Default.OrderNumberInMediaTagEnabled;
            _userSettings.OutputPath = Settings.Default.Directory;
            _userSettings.RecordUnknownTrackTypeEnabled = Settings.Default.RecordUnknownTrackTypeEnabled;
            _userSettings.TrackTitleSeparator = Settings.Default.TrackTitleSeparatorEnabled ? "_" : " ";

            txtRecordingNum.Text = _userSettings.InternalOrderNumber.ToString("000");

            _audioSession = new MainAudioSession(indexAudioEndPointDevice);
            SetAudioEndPointDevicesDropDown();
            UpdateAudioEndPointFields();

            var _logs = Settings.Default.Logs.Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            WritePreviousLogsIntoConsole(_logs);

            var lastVersionPrompted = Settings.Default.LastVersionPrompted.ToVersion();
            lnkRelease.Visible = lastVersionPrompted != null && lastVersionPrompted > Assembly.GetExecutingAssembly().GetName().Version;

            ResumeLayout();

            GitHub.GetVersion(this);
        }

        private void SetMediaTagsAPI(MediaTagsAPI api, bool isSpotifyAPISet)
        {
            switch(api)
            {
                case MediaTagsAPI.Spotify:
                    if (isSpotifyAPISet)
                    {
                        ExternalAPI.Instance = new MediaTags.SpotifyAPI(_userSettings.SpotifyAPIClientId, _userSettings.SpotifyAPISecretId);
                    }
                    break;
                case MediaTagsAPI.LastFM:
                default:
                    ExternalAPI.Instance = new MediaTags.LastFMAPI();
                    break;
            }
        }

        private void UpdateAudioEndPointFields()
        {
            tbVolumeWin.Value = _audioSession.AudioDeviceVolume;
            lblSoundCard.Text = _audioSession.AudioEndPointDevice.FriendlyName;
            lblVolume.Text = _audioSession.AudioDeviceVolume + @"%";
        }

        private void SetAudioEndPointDevicesDropDown()
        {
            var audioEndPointDevices = new Dictionary<int, string> { };

            for (var i = 0; i < _audioSession.AudioEndPointDevices.Count; i++)
            {
                audioEndPointDevices.Add(i, _audioSession.AudioEndPointDevices[i].FriendlyName);
            }

            var defaultEndPointDeviceIndex = audioEndPointDevices.FirstOrDefault(x => x.Value == _audioSession.DefaultEndPointDevice.FriendlyName).Key;
            var selectedIndex = _audioSession.IsAudioEndPointDeviceIndexAvailable() ? _audioSession.AudioEndPointDeviceIndex ?? 0 : defaultEndPointDeviceIndex;

            cbAudioDevices.DataSource = new BindingSource(audioEndPointDevices, null);
            cbAudioDevices.DisplayMember = "Value";
            cbAudioDevices.ValueMember = "Key";
            cbAudioDevices.SelectedIndex = selectedIndex;
        }

        private void SetLanguageDropDown()
        {
            cbLanguage.DataSource = new BindingSource(Translations.Languages.dropdownListValues, null);
            cbLanguage.DisplayMember = "Value";
            cbLanguage.ValueMember = "Key";
        }

        private void SetLanguage(LanguageType languageType)
        {
            var rmLanguage = Translations.Languages.getResourcesManagerLanguageType(languageType);
            Rm = new ResourceManager(rmLanguage ?? typeof(Translations.en));

            tabRecord.Text = Rm.GetString(I18nKeys.TabRecord);
            tabSettings.Text = Rm.GetString(I18nKeys.TabSettings);
            tabAdvanced.Text = Rm.GetString(I18nKeys.TabAdvanced);

            folderBrowserDialog.Description = Rm.GetString(I18nKeys.MsgFolderDialog);

            lblPath.Text = Rm.GetString(I18nKeys.LblPath);
            lblAudioDevice.Text = Rm.GetString(I18nKeys.LblAudioDevice);
            lblBitRate.Text = Rm.GetString(I18nKeys.LblBitRate);
            lblFormat.Text = Rm.GetString(I18nKeys.LblFormat);
            lblMinLength.Text = Rm.GetString(I18nKeys.LblMinLength);
            lblLanguage.Text = Rm.GetString(I18nKeys.LblLanguage);
            lblAddFolders.Text = Rm.GetString(I18nKeys.LblAddFolders);
            lblAddSeparators.Text = Rm.GetString(I18nKeys.LblAddSeparators);
            lblNumFiles.Text = Rm.GetString(I18nKeys.LblNumFiles);
            lblNumTracks.Text = Rm.GetString(I18nKeys.LblNumTracks);
            lblEndingSongDelay.Text = Rm.GetString(I18nKeys.LblEndingSongDelay);
            lblRecordingNum.Text = Rm.GetString(I18nKeys.LblRecordingNum);
            lblAds.Text = Rm.GetString(I18nKeys.LblAds);
            lblMuteAds.Text = Rm.GetString(I18nKeys.LblMuteAds);
            lblSpy.Text = Rm.GetString(I18nKeys.LblSpy);
            lblRecorder.Text = Rm.GetString(I18nKeys.LblRecorder);
            lblRecordUnknownTrackType.Text = Rm.GetString(I18nKeys.LblRecordUnknownTrackType);
            lblDuplicateAlreadyRecordedTrack.Text = Rm.GetString(I18nKeys.LblDuplicateAlreadyRecordedTrack);
            lblRecordingTimer.Text = Rm.GetString(I18nKeys.LblRecordingTimer);

            tip.SetToolTip(lnkClear, Rm.GetString(I18nKeys.TipClear));
            tip.SetToolTip(lnkSpy, Rm.GetString(I18nKeys.TipStartSpying));
            tip.SetToolTip(lnkDirectory, Rm.GetString(I18nKeys.TipDirectory));
            tip.SetToolTip(lnkPath, Rm.GetString(I18nKeys.TipPath));
            tip.SetToolTip(lnkRelease, Rm.GetString(I18nKeys.TipRelease));

            var bitrates = new Dictionary<LAMEPreset, string>
            {
                {LAMEPreset.ABR_128, Rm.GetString(I18nKeys.CbOptBitRate128)},
                {LAMEPreset.ABR_160, string.Format(Rm.GetString(I18nKeys.CbOptBitRateSpotifyFree) ?? "{0}", Rm.GetString(I18nKeys.CbOptBitRate160))},
                {LAMEPreset.ABR_256, Rm.GetString(I18nKeys.CbOptBitRate256)},
                {LAMEPreset.ABR_320, string.Format(Rm.GetString(I18nKeys.CbOptBitRateSpotifyPremium) ?? "{0}", Rm.GetString(I18nKeys.CbOptBitRate320))}
            };

            cbBitRate.DataSource = new BindingSource(bitrates, null);
            cbBitRate.DisplayMember = "Value";
            cbBitRate.ValueMember = "Key";
        }

        public void UpdateNum(int num)
        {
            if (txtRecordingNum.InvokeRequired)
            {
                var x = BeginInvoke(new Action(() => UpdateNum(num)));
                x.AsyncWaitHandle.WaitOne();
                EndInvoke(x);
                return;
            }

            txtRecordingNum.Text = num.ToString("000");
        }

        public void UpdateStartButton()
        {
            if (lnkSpy.InvokeRequired)
            {
                BeginInvoke(new Action(UpdateStartButton));
                return;
            }

            tip.SetToolTip(lnkSpy, Rm.GetString(I18nKeys.TipStartSpying));
            lnkSpy.Image = Resources.on;
            lnkSpy.Focus();
        }

        public void UpdateIconSpotify(bool isSpotifyPlaying, bool isRecording = false)
        {
            if (iconSpotify.InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateIconSpotify(isSpotifyPlaying, isRecording)));
                return;
            }

            if (isRecording)
            {
                iconSpotify.BackgroundImage = Resources.record;
                Task.Run(async () => await _analytics.LogAction("record"));
            }
            else if (isSpotifyPlaying)
            {
                iconSpotify.BackgroundImage = Resources.play;
                Task.Run(async () => await _analytics.LogAction("play"));
            }
            else
            {
                iconSpotify.BackgroundImage = Resources.pause;
                Task.Run(async () => await _analytics.LogAction("pause"));
            }
        }

        public void UpdatePlayingTitle(string text)
        {
            if (lblPlayingTitle.InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdatePlayingTitle(text)));
                return;
            }

            lblPlayingTitle.Text = text;
        }

        public void UpdateRecordedTime(int? time)
        {
            if (lblRecordedTime.InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateRecordedTime(time)));
                return;
            }

            lblRecordedTime.Text = time.HasValue ? TimeSpan.FromSeconds(time.Value).ToString(@"mm\:ss") : "";
        }

        private string WriteRtbLine(RichTextBox rtbLog, string text)
        {
            var log = "";

            if (text == null) return log;
             
            var timeStr = LogDate;
            var indexOfColon = text.IndexOf(':');
            var alert = text[0] == '/' || indexOfColon == -1;

            rtbLog.AppendText(timeStr);

            if (!alert)
            {
                var isDeleting = Regex.IsMatch(text, @"\[< \d+s\]");
                var attrb = text.Substring(0, indexOfColon);
                var msg = text.Substring(indexOfColon, text.Length - indexOfColon);
                rtbLog.AppendText(attrb);
                rtbLog.Select(rtbLog.TextLength - attrb.Length, attrb.Length + 1);
                rtbLog.SelectionColor = Color.White;
                rtbLog.AppendText(msg + Environment.NewLine);
                rtbLog.Select(rtbLog.TextLength - msg.Length, msg.Length);
                rtbLog.SelectionColor = isDeleting ? Color.IndianRed : Color.SpringGreen;

                log = $";{timeStr}{attrb}{msg}";
            }
            else
            {
                rtbLog.AppendText(text + Environment.NewLine);
            }

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.ScrollToCaret();

            return log;
        }

        public void WriteIntoConsole(TranslationKeys resource, params object[] args)
        {
            if (rtbLog.InvokeRequired)
            {
                BeginInvoke(new Action(() => WriteIntoConsole(resource, args)));
                return;
            }

            var formatted = string.Format(Rm.GetString(resource), args);
            var log = WriteRtbLine(rtbLog, formatted);

            if (!string.IsNullOrEmpty(log))
            {
                Settings.Default.Logs += $";{log}";
                Settings.Default.Save();
            }
        }

        private void WritePreviousLogsIntoConsole(string[] logs)
        {
            if (logs.Length == 0) return;

            foreach(var log in logs)
            {
                rtbLog.AppendText(log + Environment.NewLine);
            }

            rtbLog.AppendText(LogDate + Rm.GetString(I18nKeys.LogPreviousLogs) + Environment.NewLine + Environment.NewLine);

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.ScrollToCaret();
        }

        private void StartRecording()
        {
            _watcher = new Watcher(this, _userSettings);

            var watcherTask = new Task(async () => await _watcher.Run());
            watcherTask.Start();

            tip.SetToolTip(lnkSpy, Rm.GetString(I18nKeys.TipStopSying));
            tlSettings.Enabled = false;
            tlAdvanced.Enabled = false;
            timer1.Start();
        }

        public void StopRecording()
        {
            if (tlSettings.InvokeRequired || tlAdvanced.InvokeRequired)
            {
                BeginInvoke(new Action(StopRecording));
                return;
            }
            
            Watcher.Running = false;
            _toggleStopRecordingDelayed = false;
            timer1.Stop();
            tlSettings.Enabled = true;
            tlAdvanced.Enabled = true;
        }

        private bool DirExists()
        {
            if (Directory.Exists(_userSettings.OutputPath)) return true;

            MetroMessageBox.Show(this,
                Rm.GetString(I18nKeys.MsgBodyPathNotFound),
                Rm.GetString(I18nKeys.MsgTitlePathNotFound),
                MessageBoxButtons.OK,
                MessageBoxIcon.Question);

            return false;
        }

        private void LnkSpy_Click(object sender, EventArgs e)
        {
            if (!Watcher.Running)
            {
                if (!DirExists()) return;

                tcMenu.SelectedIndex = 0;
                StartRecording();
                UpdateLnkSpyIconWith(Resources.off);
                Task.Run(async () => await _analytics.LogAction("recording-session?status=started"));
            }
            else if (_watcher.RecorderUpAndRunning && !_toggleStopRecordingDelayed)
            {
                _toggleStopRecordingDelayed = true;
                Watcher.ToggleStopRecordingDelayed = _toggleStopRecordingDelayed;
            }
            else
            {
                StopRecording();
                UpdateLnkSpyIconWith(Resources.on);
                Task.Run(async () => await _analytics.LogAction("recording-session?status=ended"));
            }
        }

        private void UpdateLnkSpyIconWith(Bitmap bmp)
        {
            lnkSpy.Image.Dispose();
            lnkSpy.Image = bmp;
            lnkSpy.Refresh();
        }

        private void LnkClear_Click(object sender, EventArgs e)
        {
            rtbLog.Text = "";
            Task.Run(async () => await _analytics.LogAction("clear-console"));
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (_watcher == null) return;
            _watcher.CountSeconds++;

            if (!Watcher.Running && !Watcher.Ready)
            {
                StopRecording();
            }
        }

        private void RbFormat_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (!rb.Checked) return;
            var mediaFormat = rb?.Tag?.ToString().ToMediaFormat() ?? MediaFormat.Mp3;
            _userSettings.MediaFormat = mediaFormat;
            tlpAPI.Visible = mediaFormat == MediaFormat.Mp3;
            Settings.Default.MediaFormat = (int)(rbMp3.Checked ? MediaFormat.Mp3 : MediaFormat.Wav);
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"media-format?type={mediaFormat.ToString()}"));
        }

        private void RbLastFMAPI_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (!rb.Checked) return;
            var api = rb?.Tag?.ToString().ToMediaTagsAPI() ?? MediaTagsAPI.LastFM;
            SetMediaTagsAPI(api, _userSettings.IsSpotifyAPISet);
            Settings.Default.MediaTagsAPI = (int)(rbLastFMAPI.Checked ? MediaTagsAPI.LastFM : MediaTagsAPI.Spotify);
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"media-tags-api?type={api.ToString()}"));
        }

        private void TgRecordUnkownTrackType_CheckedChanged(object sender, EventArgs e)
        {
            _userSettings.RecordUnknownTrackTypeEnabled = tgRecordUnkownTrackType.Checked;
            Settings.Default.RecordUnknownTrackTypeEnabled = tgRecordUnkownTrackType.Checked;
            Settings.Default.Save();

            if (tgRecordUnkownTrackType.Checked)
            {
                tgMuteAds.Checked = false;
            }

            Task.Run(async () => await _analytics.LogAction($"record-unknown-type?enabled={tgRecordUnkownTrackType.Checked}"));
        }

        private void TgMuteAds_CheckedChanged(object sender, EventArgs e)
        {
            _userSettings.MuteAdsEnabled = tgMuteAds.Checked;
            Settings.Default.MuteAdsEnabled = tgMuteAds.Checked;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"mute-ads?enabled={tgMuteAds.Checked}"));
        }

        private void TgEndingSongDelay_CheckedChanged(object sender, EventArgs e)
        {
            _userSettings.EndingTrackDelayEnabled = tgEndingSongDelay.Checked;
            Settings.Default.EndingSongDelayEnabled = tgEndingSongDelay.Checked;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"delay-on-ending-song?enabled={tgEndingSongDelay.Checked}"));
        }

        private void TgAddFolders_CheckedChanged(object sender, EventArgs e)
        {
            _userSettings.GroupByFoldersEnabled = tgAddFolders.Checked;
            Settings.Default.GroupByFoldersEnabled = tgAddFolders.Checked;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"group-by-folders?enabled={tgAddFolders.Checked}"));
        }

        private void TgAddSeparators_CheckedChanged(object sender, EventArgs e)
        {
            _userSettings.TrackTitleSeparator = tgAddSeparators.Checked ? "_" : " ";
            Settings.Default.TrackTitleSeparatorEnabled = tgAddSeparators.Checked;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"track-title-separator?enabled={tgAddSeparators.Checked}"));
        }

        private void TgNumFiles_CheckedChanged(object sender, EventArgs e)
        {
            _userSettings.OrderNumberInfrontOfFileEnabled = tgNumFiles.Checked;
            Settings.Default.OrderNumberInfrontOfFileEnabled = tgNumFiles.Checked;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"order-number-in-front-of-files?enabled={tgNumFiles.Checked}"));
        }

        private void TgNumTracks_CheckedChanged(object sender, EventArgs e)
        {
            _userSettings.OrderNumberInMediaTagEnabled = tgNumTracks.Checked;
            Settings.Default.OrderNumberInMediaTagEnabled = tgNumTracks.Checked;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"order-number-in-media-tags?enabled={tgNumTracks.Checked}"));
        }

        private void TgDuplicateAlreadyRecordedTrack_CheckedChanged(object sender, EventArgs e)
        {
            _userSettings.DuplicateAlreadyRecordedTrack = tgDuplicateAlreadyRecordedTrack.Checked;
            Settings.Default.DuplicateAlreadyRecordedTrack = tgDuplicateAlreadyRecordedTrack.Checked;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"duplicate-already-recorded-track?enabled={tgDuplicateAlreadyRecordedTrack.Checked}"));
        }

        private void FrmEspionSpotify_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Watcher.Ready || !Watcher.Running) return;
            e.Cancel = true;
            if (MetroMessageBox.Show(this,
                    Rm.GetString(I18nKeys.MsgBodyCantQuit),
                    Rm.GetString(I18nKeys.MsgTitleCantQuit),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes) return;
            Watcher.Running = false;
            Thread.Sleep(1000);
            Task.Run(async () => await _analytics.LogAction("exit"));
            Close();
        }

        private void LnkPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = string.IsNullOrEmpty(txtPath.Text)
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
                : Path.GetDirectoryName(txtPath.Text);

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void TxtPath_TextChanged(object sender, EventArgs e)
        {
            _userSettings.OutputPath = txtPath.Text;
            Settings.Default.Directory = txtPath.Text;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction("set-output-folder"));
        }

        private void CbBitRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            _userSettings.Bitrate = ((KeyValuePair<LAMEPreset, string>)cbBitRate.SelectedItem).Key;
            Settings.Default.Bitrate = cbBitRate.SelectedIndex;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"bitrate?selected={cbBitRate.SelectedValue}"));
        }

        private void CbAudioDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            _userSettings.AudioEndPointDeviceIndex = ((KeyValuePair<int, string>)cbAudioDevices.SelectedItem).Key;
            _audioSession = new MainAudioSession(cbAudioDevices.SelectedIndex);
            UpdateAudioEndPointFields();
            Settings.Default.AudioEndPointDeviceIndex = cbAudioDevices.SelectedIndex.ToString();
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"audioEndPointDevice?selected={cbAudioDevices.SelectedValue}"));
        }

        private void LnkNumMinus_Click(object sender, EventArgs e)
        {
            if (_userSettings.InternalOrderNumber - 1 >= 0)
            {
                _userSettings.InternalOrderNumber--;
            }

            txtRecordingNum.Text = _userSettings.InternalOrderNumber.ToString("000");
        }

        private void LnkNumPlus_Click(object sender, EventArgs e)
        {
            if (_userSettings.InternalOrderNumber + 1 < 1000)
            {
                _userSettings.InternalOrderNumber++;
            }

            txtRecordingNum.Text = _userSettings.InternalOrderNumber.ToString("000");
        }

        private void LnkDirectory_Click(object sender, EventArgs e)
        {
            if (DirExists())
            {
                System.Diagnostics.Process.Start("explorer.exe", txtPath.Text);
            }
            Task.Run(async () => await _analytics.LogAction("open-output-folder"));
        }

        private void TbMinTime_ValueChanged(object sender, EventArgs e)
        {
            _userSettings.MinimumRecordedLengthSeconds = tbMinTime.Value * 5;
            var min = (_userSettings.MinimumRecordedLengthSeconds / 60);
            var sec = (_userSettings.MinimumRecordedLengthSeconds % 60);
            lblMinTime.Text = min + @":" + sec.ToString("00");
            Settings.Default.MinimumRecordedLengthSeconds = tbMinTime.Value * 5;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"minimum-media-time?value={tbMinTime.Value}"));
        }

        private void TbVolumeWin_ValueChanged(object sender, EventArgs e)
        {
            if (_audioSession.AudioEndPointDevice.AudioEndpointVolume.Mute)
            {
                _audioSession.AudioEndPointDevice.AudioEndpointVolume.Mute = false;
            }

            _audioSession.SetDefaultAudioDeviceVolume(tbVolumeWin.Value);
            lblVolume.Text = (tbVolumeWin.Value) + @"%";

            if (tbVolumeWin.Value == 0)
            {
                if (iconVolume.BackgroundImage != Resources.volmute) iconVolume.BackgroundImage = Resources.volmute;
            }
            else if (tbVolumeWin.Value > 0 && tbVolumeWin.Value < 30)
            {
                if (iconVolume.BackgroundImage != Resources.voldown) iconVolume.BackgroundImage = Resources.voldown;
            }
            else
            {
                if (iconVolume.BackgroundImage != Resources.volup) iconVolume.BackgroundImage = Resources.volup;
            }

            Task.Run(async () => await _analytics.LogAction("volume"));
        }

        private void Focus_Hover(object sender, EventArgs e)
        {
            var ctrl = (Control) sender;
            ctrl.Focus();
        }

        private void CbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.Language = cbLanguage.SelectedIndex;
            Settings.Default.Save();

            var language = (LanguageType)cbLanguage.SelectedIndex;
            SetLanguage(language);
            Task.Run(async () => await _analytics.LogAction($"language?selected={cbLanguage.SelectedValue}"));
        }

        private void TcMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.TabNo = tcMenu.SelectedIndex;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"tab?selected={tcMenu.SelectedTab}"));
        }

        private void LnkRelease_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/jwallet/spy-spotify/releases/latest");
        }

        private void TxtRecordingTimer_Leave(object sender, EventArgs e)
        {
            _userSettings.RecordingTimer = txtRecordingTimer.Text;
        }

        private void TxtRecordingNum_Leave(object sender, EventArgs e)
        {
            _userSettings.InternalOrderNumber = int.Parse(txtRecordingNum.Text);
        }
    }
}
