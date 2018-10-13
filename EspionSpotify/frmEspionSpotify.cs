using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Resources;
using EspionSpotify.Ads;
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

namespace EspionSpotify
{
    public sealed partial class FrmEspionSpotify : MetroForm
    {
        private readonly IMainAudioSession _audioSession;
        private Watcher _watcher;
        private UserSettings _userSettings;
        private Analytics _analytics;

        public static ResourceManager Rm;
        public static FrmEspionSpotify Instance;

        public FrmEspionSpotify()
        {
            SuspendLayout();

            Instance = this;
            InitializeComponent();

            _userSettings = new UserSettings();
            Rm = new ResourceManager(typeof(english));
            BackImage = Resources.spytify_logo;

            if (Settings.Default.Directory.Equals(string.Empty))
            {
                Settings.Default.Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                Settings.Default.Save();
            }

            if (Settings.Default.AnalyticsCID.Equals(string.Empty))
            {
                Settings.Default.AnalyticsCID = Analytics.GenerateCID();
                Settings.Default.Save();
            }
            _analytics = new Analytics(Settings.Default.AnalyticsCID, Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Task.Run(async () => await _analytics.LogAction("launch"));

            var indexLanguage = Settings.Default.Language;
            var indexBitRate = Settings.Default.Bitrate;

            tcMenu.SelectedIndex = Settings.Default.TabNo;

            rbMp3.Checked = Settings.Default.MediaFormat == 0;
            rbWav.Checked = Settings.Default.MediaFormat == 1;
            tbMinTime.Value = Settings.Default.MinimumRecordedLengthSeconds / 5;
            tgEndingSongDelay.Checked = Settings.Default.EndingSongDelayEnabled;
            tgAddSeparators.Checked = Settings.Default.TrackTitleSeparatorEnabled;
            tgNumTracks.Checked = Settings.Default.OrderNumberInMediaTagEnabled;
            tgNumFiles.Checked = Settings.Default.OrderNumberInfrontOfFileEnabled;
            tgAddFolders.Checked = Settings.Default.GroupByFoldersEnabled;
            txtPath.Text = Settings.Default.Directory;
            tgDisableAds.Checked = ManageHosts.AreAdsDisabled(ManageHosts.HostsSystemPath);
            tgMuteAds.Checked = Settings.Default.MuteAdsEnabled;
            tgDuplicateAlreadyRecordedTrack.Checked = Settings.Default.DuplicateAlreadyRecordedTrack;
            folderBrowserDialog.SelectedPath = Settings.Default.Directory;

            SetLanguageDropDown();

            var language = (LanguageType)indexLanguage;
            SetLanguage(language);

            cbBitRate.SelectedIndex = indexBitRate;
            cbLanguage.SelectedIndex = indexLanguage;

            _audioSession = new MainAudioSession();

            tbVolumeWin.Value = _audioSession.DefaultAudioDeviceVolume;
            lblSoundCard.Text = _audioSession.DefaultAudioEndPointDevice.FriendlyName;
            lblVolume.Text = _audioSession.DefaultAudioDeviceVolume + @"%";

            _userSettings.OutputPath = Settings.Default.Directory;
            _userSettings.Bitrate = ((KeyValuePair<LAMEPreset, string>)cbBitRate.SelectedItem).Key;
            _userSettings.MediaFormat = (MediaFormat)Settings.Default.MediaFormat;
            _userSettings.MinimumRecordedLengthSeconds = Settings.Default.MinimumRecordedLengthSeconds;
            _userSettings.GroupByFoldersEnabled = Settings.Default.GroupByFoldersEnabled;
            _userSettings.TrackTitleSeparator = Settings.Default.TrackTitleSeparatorEnabled ? "_" : " ";
            _userSettings.OrderNumberInMediaTagEnabled = Settings.Default.OrderNumberInMediaTagEnabled;
            _userSettings.OrderNumberInfrontOfFileEnabled = Settings.Default.OrderNumberInfrontOfFileEnabled;
            _userSettings.EndingTrackDelayEnabled = Settings.Default.EndingSongDelayEnabled;
            _userSettings.DuplicateAlreadyRecordedTrack = Settings.Default.DuplicateAlreadyRecordedTrack;
            _userSettings.InternalOrderNumber = 1;

            var lastVersionPrompted = Settings.Default.LastVersionPrompted.ToVersion();
            lnkRelease.Visible = lastVersionPrompted != null && lastVersionPrompted > Assembly.GetExecutingAssembly().GetName().Version;

            ResumeLayout();

            GitHub.GetVersion();
        }

        private void SetLanguageDropDown()
        {
            var langs = new Dictionary<LanguageType, string>
            {
                {LanguageType.En, Rm.GetString($"cbOptLangEn")},
                {LanguageType.Fr, Rm.GetString($"cbOptLangFr")}
            };

            cbLanguage.DataSource = new BindingSource(langs, null);
            cbLanguage.DisplayMember = "Value";
            cbLanguage.ValueMember = "Key";
        }

        private void SetLanguage(LanguageType languageType)
        {
            Rm = languageType == LanguageType.Fr ? new ResourceManager(typeof(french)) : new ResourceManager(typeof(english));

            tabRecord.Text = Rm.GetString($"tabRecord");
            tabSettings.Text = Rm.GetString($"tabSettings");
            tabAdvanced.Text = Rm.GetString($"tabAdvanced");
            tabFAQ.Text = Rm.GetString($"tabFAQ");

            lblPath.Text = Rm.GetString($"lblPath");
            lblBitRate.Text = Rm.GetString($"lblBitRate");
            lblFormat.Text = Rm.GetString($"lblFormat");
            lblMinLength.Text = Rm.GetString($"lblMinLength");
            lblLanguage.Text = Rm.GetString($"lblLanguage");
            lblAddFolders.Text = Rm.GetString($"lblAddFolders");
            lblAddSeparators.Text = Rm.GetString($"lblAddSeparators");
            lblNumFiles.Text = Rm.GetString($"lblNumFiles");
            lblNumTracks.Text = Rm.GetString($"lblNumTracks");
            lblEndingSongDelay.Text = Rm.GetString($"lblEndingSongDelay");
            lblRecordingNum.Text = Rm.GetString($"lblRecordingNum");
            lblAds.Text = Rm.GetString($"lblAds");
            lblDisableAds.Text = Rm.GetString($"lblDisableAds");
            lblMuteAds.Text = Rm.GetString($"lblMuteAds");
            lblSpy.Text = Rm.GetString($"lblSpy");
            lblRecorder.Text = Rm.GetString($"lblRecorder");
            lblRecordUnknownTrackType.Text = Rm.GetString($"lblRecordUnknownTrackType");
            lblDuplicateAlreadyRecordedTrack.Text = Rm.GetString($"lblDuplicateAlreadyRecordedTrack");

            tlSpotifyTrackCut.Text = Rm.GetString($"tlSpotifyTrackCut");
            lblSpotifyTrackCut.Text = Rm.GetString($"lblSpotifyTrackCut");
            tlAdsPlayAndStop.Text = Rm.GetString($"tlAdsPlayAndStop");
            lblAdsPlayAndStop.Text = Rm.GetString($"lblAdsPlayAndStop");
            tlAdAndTrackOverlapOnRecordedTrack.Text = Rm.GetString($"tlAdAndTrackOverlapOnRecordedTrack");
            lblAdAndTrackOverlapOnRecordedTrack.Text = Rm.GetString($"lblAdAndTrackOverlapOnRecordedTrack");
            tlBackgroundNoiceRecordedOnTrack.Text = Rm.GetString($"tlBackgroundNoiceRecordedOnTrack");
            lblBackgroundNoiceRecordedOnTrack.Text = Rm.GetString($"lblBackgroundNoiceRecordedOnTrack");
            tlTrackDetectedAsAd.Text = Rm.GetString($"tlTrackDetectedAsAd");
            lblTrackDetectedAsAd.Text = Rm.GetString($"lblTrackDetectedAsAd");
            tlSpotifyLostFeatures.Text = Rm.GetString($"tlSpotifyLostFeatures");
            lblSpotifyLostFeatures.Text = Rm.GetString($"lblSpotifyLostFeatures");

            tip.SetToolTip(lnkClear, Rm.GetString($"tipClear"));
            tip.SetToolTip(lnkSpy, Rm.GetString($"tipStartSpying"));
            tip.SetToolTip(lnkDirectory, Rm.GetString($"tipDirectory"));
            tip.SetToolTip(lnkPath, Rm.GetString($"tipPath"));
            tip.SetToolTip(lnkRelease, Rm.GetString($"tipRelease"));

            var bitrates = new Dictionary<LAMEPreset, string>
            {
                {LAMEPreset.ABR_128, Rm.GetString($"cbOptBitRate128")},
                {LAMEPreset.ABR_160, string.Format(Rm.GetString($"cbOptBitRateSpotifyFree") ?? "{0}", Rm.GetString($"cbOptBitRate160"))},
                {LAMEPreset.ABR_256, Rm.GetString($"cbOptBitRate256")},
                {LAMEPreset.ABR_320, string.Format(Rm.GetString($"cbOptBitRateSpotifyPremium") ?? "{0}", Rm.GetString($"cbOptBitRate320"))}
            };

            cbBitRate.DataSource = new BindingSource(bitrates, null);
            cbBitRate.DisplayMember = "Value";
            cbBitRate.ValueMember = "Key";
        }

        public void UpdateNum(int num)
        {
            if (lblNum.InvokeRequired)
            {
                var x = BeginInvoke(new Action(() => UpdateNum(num)));
                x.AsyncWaitHandle.WaitOne();
                EndInvoke(x);
                return;
            }

            lblNum.Text = num.ToString("000");
        }

        public void UpdateStartButton()
        {
            if (lnkSpy.InvokeRequired)
            {
                BeginInvoke(new Action(UpdateStartButton));
                return;
            }

            tip.SetToolTip(lnkSpy, Rm.GetString($"tipStartSpying"));
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

        public void WriteIntoConsole(string text)
        {
            if (rtbLog.InvokeRequired)
            {
                BeginInvoke(new Action(() => WriteIntoConsole(text)));
                return;
            }

            if (text != null)
            {
                var timeStr = $@"[{DateTime.Now:HH:mm:ss}] ";
                var alert = text[0] == '/';
                
                rtbLog.AppendText(timeStr);
                if (!alert)
                {
                    var indexOfColon = text.IndexOf(':');
                    var attrb = text.Substring(0, indexOfColon);
                    var msg = text.Substring(indexOfColon, text.Length - indexOfColon);
                    rtbLog.AppendText(attrb);
                    rtbLog.Select(rtbLog.TextLength - attrb.Length, attrb.Length + 1);
                    rtbLog.SelectionColor = Color.White;
                    rtbLog.AppendText(msg + Environment.NewLine);
                    rtbLog.Select(rtbLog.TextLength - msg.Length, msg.Length);
                    rtbLog.SelectionColor = Color.SpringGreen;
                }
                else
                {
                    rtbLog.AppendText(text + Environment.NewLine);
                }

                rtbLog.SelectionStart = rtbLog.TextLength;
                rtbLog.ScrollToCaret();
            }
        }

        private void StartRecording()
        {
            _watcher = new Watcher(this, _userSettings);

            var watcherThread = new Thread(async () => await _watcher.Run());
            watcherThread.Start();

            tip.SetToolTip(lnkSpy, Rm.GetString($"tipStopSying"));
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
            timer1.Stop();
            tlSettings.Enabled = true;
            tlAdvanced.Enabled = true;
        }

        private bool DirExists()
        {
            if (Directory.Exists(_userSettings.OutputPath)) return true;

            MetroMessageBox.Show(this,
                Rm.GetString($"msgBodyPathNotFound"),
                Rm.GetString($"msgTitlePathNotFound"),
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
                lnkSpy.Image = Resources.off;
                Task.Run(async () => await _analytics.LogAction("recording-session?status=started"));
            }
            else
            {
                StopRecording();
                lnkSpy.Image = Resources.on;
                Task.Run(async () => await _analytics.LogAction("recording-session?status=ended"));
            }
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

            if (!Watcher.Running) StopRecording();
        }

        private void RbFormat_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            _userSettings.MediaFormat = rb != null && rb.Tag.ToString() == "mp3" ? MediaFormat.Mp3 : MediaFormat.Wav;
            Settings.Default.MediaFormat = (rbMp3.Checked ? 0 : 1);
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"media-format?type={rb.Tag}"));
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

        private void TgDisableAds_CheckedChanged(object sender, EventArgs e)
        {
            if (!ManageSpotifyAds(tgDisableAds.Checked)) return;

            Settings.Default.DisableAds = tgDisableAds.Checked;
            Settings.Default.Save();
        }

        private void TgDisableAds_Click(object sender, EventArgs e)
        {
            Task.Run(async () => await _analytics.LogAction($"disable-ads?enabled={tgDisableAds.Checked}"));

            if (Administrator.EnsureAdmin())
            {
                // recover friend list, radios, podcasts, daily mixes : spclient.wg.spotify.com
                if (tgDisableAds.Checked && MetroMessageBox.Show(this,
                    Rm.GetString($"msgBodyDisableAds"),
                    Rm.GetString($"msgTitleDisableAds"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Question) == DialogResult.OK) return;
                return;
            }

            tgDisableAds.Checked = !tgDisableAds.Checked;
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
                    Rm.GetString($"msgBodyCantQuit"),
                    Rm.GetString($"msgTitleCantQuit"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes) return;
            Watcher.Running = false;
            Thread.Sleep(1000);
            Task.Run(async () => await _analytics.LogAction("exit"));
            Close();
        }

        private void LnkPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = txtPath.Text.Equals(string.Empty)
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

        private void LnkNumMinus_Click(object sender, EventArgs e)
        {
            if (!_userSettings.InternalOrderNumber.HasValue) return;

            if (_userSettings.InternalOrderNumber - 1 >= 0)
            {
                _userSettings.InternalOrderNumber--;
            }

            lblNum.Text = (_userSettings.InternalOrderNumber ?? 0).ToString("000");
        }

        private void LnkNumPlus_Click(object sender, EventArgs e)
        {
            if (!_userSettings.InternalOrderNumber.HasValue) return;

            if (_userSettings.InternalOrderNumber + 1 < 1000)
            {
                _userSettings.InternalOrderNumber++;
            }

            lblNum.Text = (_userSettings.InternalOrderNumber ?? 0).ToString("000");
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
            if (_audioSession.DefaultAudioEndPointDevice.AudioEndpointVolume.Mute)
            {
                _audioSession.DefaultAudioEndPointDevice.AudioEndpointVolume.Mute = false;
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

        private static bool ManageSpotifyAds(bool isToggled)
        {
            return isToggled
                ? ManageHosts.DisableAds(ManageHosts.HostsSystemPath)
                : ManageHosts.EnableAds(ManageHosts.HostsSystemPath);
        }

        private void ShowHideLabel(Label label)
        {
            if (label.Visible)
            {
                label.Hide();
            }
            else
            {
                label.Show();
            }
        }

        private void TlTrackDetectedAsAd_Leave(object sender, EventArgs e)
        {
            lblTrackDetectedAsAd.Hide();
        }

        private void TlTrackDetectedAsAd_Click(object sender, EventArgs e)
        {
            ShowHideLabel(lblTrackDetectedAsAd);
            Task.Run(async () => await _analytics.LogAction($"faq?selected=track-detected-as-ad"));
        }

        private void TlBackgroundNoiceRecordedOnTrack_Leave(object sender, EventArgs e)
        {
            lblBackgroundNoiceRecordedOnTrack.Hide();
        }

        private void TlBackgroundNoiceRecordedOnTrack_Click(object sender, EventArgs e)
        {
            ShowHideLabel(lblBackgroundNoiceRecordedOnTrack);
            Task.Run(async () => await _analytics.LogAction($"faq?selected=background-noice-recorded-on-track"));
        }

        private void TlAdAndTrackOverlapOnRecordedTrack_Leave(object sender, EventArgs e)
        {
            lblAdAndTrackOverlapOnRecordedTrack.Hide();
        }

        private void TlAdAndTrackOverlapOnRecordedTrack_Click(object sender, EventArgs e)
        {
            ShowHideLabel(lblAdAndTrackOverlapOnRecordedTrack);
            Task.Run(async () => await _analytics.LogAction($"faq?selected=ad-and-track-overlap"));
        }

        private void TlAdsPlayAndStop_Leave(object sender, EventArgs e)
        {
            lblAdsPlayAndStop.Hide();
        }

        private void TlAdsPlayAndStop_Click(object sender, EventArgs e)
        {
            ShowHideLabel(lblAdsPlayAndStop);
            Task.Run(async () => await _analytics.LogAction($"faq?selected=ads-play-and-stop"));
        }

        private void TlSpotifyTrackCut_Leave(object sender, EventArgs e)
        {
            lblSpotifyTrackCut.Hide();
        }

        private void TlSpotifyTrackCut_Click(object sender, EventArgs e)
        {
            ShowHideLabel(lblSpotifyTrackCut);
            Task.Run(async () => await _analytics.LogAction($"faq?selected=spotify-track-cut"));
        }

        private void TlSpotifyLostFeatures_Leave(object sender, EventArgs e)
        {
            lblSpotifyLostFeatures.Hide();
        }

        private void TlSpotifyLostFeatures_Click(object sender, EventArgs e)
        {
            ShowHideLabel(lblSpotifyLostFeatures);
            Task.Run(async () => await _analytics.LogAction($"faq?selected=spotify-lost-features"));
        }

        private void lnkRelease_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/NHLGames/NHLGames/wiki");
        }
    }
}
