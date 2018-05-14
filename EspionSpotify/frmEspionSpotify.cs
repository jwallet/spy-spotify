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

namespace EspionSpotify
{
    public sealed partial class FrmEspionSpotify : MetroForm
    {
        private readonly VolumeWin _sound;
        private Watcher _watcher;
        private int _minTime;
        private bool _strucDossiers;
        private Recorder.Format _formatValue;
        private string _charSeparator;
        private bool _bCdTrack;
        private bool _bNumFile;
        private int _num;

        public static Random Rnd;
        public static ResourceManager Rm;
        public static FrmEspionSpotify Instance;

        public FrmEspionSpotify()
        {
            SuspendLayout();
            Instance = this;
            InitializeComponent();
            
            Rnd = new Random();
            Rm = new ResourceManager(typeof(english));
            BackImage = Resources.spytify_logo;

            if (Settings.Default.Directory == "")
            {
                Settings.Default.Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                Settings.Default.Save();
            }

            _charSeparator = Settings.Default.AddSeparators ? "_" : " ";
            _minTime = Settings.Default.MinLength;
            _formatValue = (Recorder.Format) Settings.Default.Format;
            _strucDossiers = Settings.Default.AddFolders;
            _bCdTrack = Settings.Default.AddNumsAsTrack;
            _bNumFile = Settings.Default.AddNumsInfrontFile;
            _num = 1;

            var indexLanguage = Settings.Default.Language;
            var indexBitRate = Settings.Default.Bitrate;

            tcMenu.SelectedIndex = Settings.Default.TabNo;

            rbMp3.Checked = Settings.Default.Format == 0;
            rbWav.Checked = Settings.Default.Format == 1;
            tbMinTime.Value = Settings.Default.MinLength / 5;
            tgAddSeparators.Checked = Settings.Default.AddSeparators;
            tgNumTracks.Checked = Settings.Default.AddNumsAsTrack;
            tgNumFiles.Checked = Settings.Default.AddNumsInfrontFile;
            tgAddFolders.Checked = Settings.Default.AddFolders;
            txtPath.Text = Settings.Default.Directory;
            tgDisableAds.Checked = Settings.Default.DisableAds;
            folderBrowserDialog.SelectedPath = Settings.Default.Directory;

            SetLanguageDropDown();

            var language = (LanguageType)indexLanguage;
            SetLanguage(language);

            cbBitRate.SelectedIndex = indexBitRate;
            cbLanguage.SelectedIndex = indexLanguage;

            _sound = new VolumeWin();

            tbVolumeWin.Value = _sound.DefaultAudioDeviceVolume;
            lblSoundCard.Text = _sound.DefaultAudioEndPointDevice.FriendlyName;
            lblVolume.Text = _sound.DefaultAudioDeviceVolume + @"%";

            ResumeLayout();
            GitHub.NewestVersion();
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
            lblPath.Text = Rm.GetString($"lblPath");
            lblBitRate.Text = Rm.GetString($"lblBitRate");
            lblFormat.Text = Rm.GetString($"lblFormat");
            lblMinLength.Text = Rm.GetString($"lblMinLength");
            lblCustomize.Text = Rm.GetString($"lblCustomize");
            lblLanguage.Text = Rm.GetString($"lblLanguage");
            lblAddFolders.Text = Rm.GetString($"lblAddFolders");
            lblAddSeparators.Text = Rm.GetString($"lblAddSeparators");
            lblNumFiles.Text = Rm.GetString($"lblNumFiles");
            lblNumTracks.Text = Rm.GetString($"lblNumTracks");
            lblRecordingNum.Text = Rm.GetString($"lblRecordingNum");
            lblAds.Text = Rm.GetString($"lblAds");
            lblDisableAds.Text = Rm.GetString($"lblDisableAds");

            tip.SetToolTip(lnkClear, Rm.GetString($"tipClear"));
            tip.SetToolTip(lnkSpy, Rm.GetString($"tipStartSpying"));
            tip.SetToolTip(lnkDirectory, Rm.GetString($"tipDirectory"));
            tip.SetToolTip(lnkPath, Rm.GetString($"tipPath"));

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
            }
            else
            {
                lblNum.Text = num.ToString("000");
            }
        }

        public void UpdateStartButton()
        {
            if (lnkSpy.InvokeRequired)
            {
                BeginInvoke(new Action(UpdateStartButton));
            }
            else
            {
                tip.SetToolTip(lnkSpy, Rm.GetString($"tipStartSpying"));
                lnkSpy.Image = Resources.on;
                lnkSpy.Focus();
            }
        }

        public void UpdateIconSpotify(bool isSpotifyPlaying, bool isRecording = false)
        {
            if (iconSpotify.InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateIconSpotify(isSpotifyPlaying, isRecording)));
            }
            else if (isRecording)
            {
                iconSpotify.BackgroundImage = Resources.record;
            }
            else if (isSpotifyPlaying)
            {
                iconSpotify.BackgroundImage = Resources.play;
            }
            else
            {
                iconSpotify.BackgroundImage = Resources.pause;
            }
        }

        public void UpdatePlayingTitle(string text)
        {
            if (lblPlayingTitle.InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdatePlayingTitle(text)));
            }
            else
            {
                lblPlayingTitle.Text = text;
            }
        }
      
        public void WriteIntoConsole(string text)
        {
            if(rtbLog.InvokeRequired)
            {
                BeginInvoke(new Action(() => WriteIntoConsole(text)));
            }
            else
            {
                var timeStr = $@"[{DateTime.Now:HH:mm:ss}] ";
                var alert = text[0] == '/';
                
                rtbLog.AppendText(timeStr);
                if (!alert)
                {
                    var attrb = text.Substring(0, text.IndexOf(':'));
                    var msg = text.Substring(text.IndexOf(':'), text.Length - text.IndexOf(':'));
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
            var bitrate = ((KeyValuePair<LAMEPreset, string>)cbBitRate.SelectedItem).Key;

            _watcher = new Watcher(this, txtPath.Text, bitrate, _formatValue, _sound, _minTime, 
                _strucDossiers, _charSeparator, _bCdTrack, _bNumFile, _num);

            var watcherThread = new Thread(_watcher.Run);
            watcherThread.Start();

            tip.SetToolTip(lnkSpy, Rm.GetString($"tipStopSying"));
            tabSettings.Enabled = false;
            timer1.Start();
        }

        public void StopRecording()
        {
            Watcher.Running = false;
            tabSettings.Enabled = true;
            timer1.Stop();
        }

        private bool DirExists()
        {
            if (Directory.Exists(txtPath.Text)) return true;

            MetroMessageBox.Show(this,
                Rm.GetString($"msgBodyPathNotFound"),
                Rm.GetString($"msgTitlePathNotFound"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Question);

            return false;
        }

        private void lnkSpy_Click(object sender, EventArgs e)
        {
            if(!Watcher.Running)
            {
                if (!DirExists()) return;

                tcMenu.SelectedIndex = 0;
                StartRecording();
                lnkSpy.Image = Resources.off;
            }
            else
            {
                StopRecording();
                lnkSpy.Image = Resources.on;
            }
        }

        private void lnkClear_Click(object sender, EventArgs e)
        {
            rtbLog.Text = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_watcher == null) return;
            _watcher.CountSecs++;

            if (!Watcher.Running) StopRecording();
        }

        private void rbFormat_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            _formatValue = rb != null && rb.Tag.ToString() == "mp3" ? Recorder.Format.Mp3 : Recorder.Format.Wav;
            Settings.Default.Format = (rbMp3.Checked ? 0 : 1);
            Settings.Default.Save();
        }

        private void tgAddFolders_CheckedChanged(object sender, EventArgs e)
        {
            _strucDossiers = tgAddFolders.Checked;
            Settings.Default.AddFolders = tgAddFolders.Checked;
            Settings.Default.Save();
        }

        private void tgAddSeparators_CheckedChanged(object sender, EventArgs e)
        {
            _charSeparator = tgAddSeparators.Checked ? "_" : " ";
            Settings.Default.AddSeparators = tgAddSeparators.Checked;
            Settings.Default.Save();
        }

        private void tgNumFiles_CheckedChanged(object sender, EventArgs e)
        {
            _bNumFile = tgNumFiles.Checked;
            Settings.Default.AddNumsInfrontFile = tgNumFiles.Checked;
            Settings.Default.Save();
        }

        private void frmEspionSpotify_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Watcher.Ready) return;
            e.Cancel = true;
            if (MetroMessageBox.Show(this,
                    Rm.GetString($"msgBodyCantQuit"),
                    Rm.GetString($"msgTitleCantQuit"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes) return;
            Watcher.Running = false;
            Thread.Sleep(1000);
            Close();
        }

        private void tgNumTracks_CheckedChanged(object sender, EventArgs e)
        {
            _bCdTrack = tgNumTracks.Checked;
            Settings.Default.AddNumsAsTrack = tgNumTracks.Checked;
            Settings.Default.Save();
        }

        private void lblNum_TextChanged(object sender, EventArgs e)
        {
            var val = Convert.ToInt16(lblNum.Text);
            _num = val;
        }

        private void lnkPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = txtPath.Text.Equals(string.Empty)
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
                : Path.GetDirectoryName(txtPath.Text);

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void lnkNumMinus_Click(object sender, EventArgs e)
        {
            if (_num - 1 < 0)
            {
                _num = 999;
            }
            else
            {
                _num--;
            }

            lblNum.Text = _num.ToString("000");
        }

        private void lnkNumPlus_Click(object sender, EventArgs e)
        {
            if (_num + 1 > 999)
            {
                _num = 0;
            }
            else
            {
                _num++;
            }

            lblNum.Text = _num.ToString("000");
        }

        private void lnkDirectory_Click(object sender, EventArgs e)
        {
            if (DirExists()) System.Diagnostics.Process.Start("explorer.exe", txtPath.Text);
        }

        private void tbMinTime_ValueChanged(object sender, EventArgs e)
        {
            _minTime = tbMinTime.Value * 5;
            var min = (_minTime / 60);
            var sec = (_minTime % 60);
            lblMinTime.Text = min + @":" + sec.ToString("00");
            Settings.Default.MinLength = tbMinTime.Value * 5;
            Settings.Default.Save();
        }

        private void tbVolumeWin_ValueChanged(object sender, EventArgs e)
        {
            if (_sound.DefaultAudioEndPointDevice.AudioEndpointVolume.Mute)
            {
                _sound.DefaultAudioEndPointDevice.AudioEndpointVolume.Mute = false;
            }

            _sound.SetDefaultAudioDeviceVolume(tbVolumeWin.Value);
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
        }

        private void focus_Hover(object sender, EventArgs e)
        {
            var ctrl = (Control) sender;
            ctrl.Focus();
        }

        private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.Language = cbLanguage.SelectedIndex;
            Settings.Default.Save();

            var language = (LanguageType)cbLanguage.SelectedIndex;
            SetLanguage(language);
        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.Directory = folderBrowserDialog.SelectedPath;
            Settings.Default.Save();
        }

        private void cbBitRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.Bitrate = cbBitRate.SelectedIndex;
            Settings.Default.Save();
        }

        private void tcMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.TabNo = tcMenu.SelectedIndex;
            Settings.Default.Save();
        }

        private void tgDisableAds_CheckedChanged(object sender, EventArgs e)
        {
            if (!ManageSpotifyAds(tgDisableAds.Checked)) return;

            Settings.Default.DisableAds = tgDisableAds.Checked;
            Settings.Default.Save();
        }

        private static bool ManageSpotifyAds(bool isToggled)
        {
            return isToggled
                ? ManageHosts.DisableAds(ManageHosts.HostsSystemPath)
                : ManageHosts.EnableAds(ManageHosts.HostsSystemPath);
        }

        private void tgDisableAds_Click(object sender, EventArgs e)
        {
            if (!Administrator.EnsureAdmin())
            {
                tgDisableAds.Checked = !tgDisableAds.Checked;
            }
        }
    }
}
