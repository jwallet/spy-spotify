using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using EspionSpotify.Properties;
using MetroFramework.Forms;
using NAudio.Lame;

namespace EspionSpotify
{
    public partial class FrmEspionSpotify : MetroForm
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

        public FrmEspionSpotify()
        {
            InitializeComponent();

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

            rbMp3.Checked = (Settings.Default.Format == 0);
            tbMinTime.Value = Settings.Default.MinLength / 5;
            tgAddSeparators.Checked = Settings.Default.AddSeparators;
            tgNumTracks.Checked = Settings.Default.AddNumsAsTrack;
            tgNumFiles.Checked = Settings.Default.AddNumsInfrontFile;
            tgAddFolders.Checked = Settings.Default.AddFolders;
            txtPath.Text = Settings.Default.Directory;
            folderBrowserDialog.SelectedPath = Settings.Default.Directory;

            tip.SetToolTip(lnkClear, "Effacer l'historique");
            tip.SetToolTip(lnkSpy, "Débuter l'espionnage");
            tip.SetToolTip(lnkDirectory, "Ouvrir le répertoire de sauvegarde");
            tip.SetToolTip(lnkPath, "Parcourir");

            var bitrates = new Dictionary<LAMEPreset, string>
            {
                {LAMEPreset.ABR_128, "128kbps"},
                {LAMEPreset.ABR_160, "160kbps (Spotify Free)"},
                {LAMEPreset.ABR_256, "256kbps"},
                {LAMEPreset.ABR_320, "320kbps Haute qualité (Spotify Premium)"}
            };

            cbBitRate.DataSource = new BindingSource(bitrates, null);
            cbBitRate.DisplayMember = "Value";
            cbBitRate.ValueMember = "Key";

            cbBitRate.SelectedIndex = Settings.Default.Bitrate;
            
            _sound = new VolumeWin();

            tbVolumeWin.Value = _sound.DefaultAudioDeviceVolume;
            lblSoundCard.Text = _sound.DefaultAudioEndPointDevice.FriendlyName;
            lblVolume.Text = _sound.DefaultAudioDeviceVolume + @"%";
        }

        private void SaveSettings()
        {
            Settings.Default.Directory = folderBrowserDialog.SelectedPath;
            Settings.Default.MinLength = tbMinTime.Value * 5;
            Settings.Default.Format = rbWav.Checked ? 1 : 0;
            Settings.Default.AddSeparators = tgAddSeparators.Checked;
            Settings.Default.AddNumsAsTrack = tgNumTracks.Checked;
            Settings.Default.AddNumsInfrontFile = tgNumFiles.Checked;
            Settings.Default.AddFolders = tgAddFolders.Checked;
            Settings.Default.Bitrate = cbBitRate.SelectedIndex;
            Settings.Default.Save();
        }

        public void UpdateNum(int num)
        {
            if (lblNum.InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateNum(num)));
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
                lnkSpy.Image = Resources.on;
                lnkSpy.Focus();
            }
        }

        public void UpdateIconSpotify(bool pause = false)
        {
            if (iconSpotify.InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateIconSpotify(pause)));
            }
            else
            {
                iconSpotify.BackgroundImage = pause ? Resources.pause : Resources.play;
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
                var commercial = !alert && text.IndexOf(':') == 9;
                
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
                    rtbLog.SelectionColor = commercial ? Color.White : Color.SpringGreen;
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
            SaveSettings();

            var bitrate = ((KeyValuePair<LAMEPreset, string>)cbBitRate.SelectedItem).Key;

            _watcher = new Watcher(this, txtPath.Text, bitrate, _formatValue, _sound, _minTime, 
                _strucDossiers, _charSeparator, _bCdTrack, _bNumFile, _num);

            var watcherThread = new Thread(_watcher.Run);
            watcherThread.Start();

            tip.SetToolTip(lnkSpy, "Arrêter l'espionnage");
            tabSettings.Enabled = false;
            timer1.Start();
        }

        public void StopRecording()
        {
            Watcher.Running = false;

            tip.SetToolTip(lnkSpy, "Débuter l'espionnage");
            tabSettings.Enabled = true;
            timer1.Stop();
        }

        private bool DirExists()
        {
            if (Directory.Exists(txtPath.Text)) return true;

            MessageBox.Show(
                @"Le répertoire de sauvegarde sélectionné n'existe pas.",
                @"Répertoire invalide",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }

        private void lnkSpy_Click(object sender, EventArgs e)
        {
            if(!Watcher.Running)
            {
                if (!DirExists()) return;

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
            _watcher.CountSecs++;

            if (!Watcher.Running) StopRecording();
        }

        private void rbFormat_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            _formatValue = rb != null && rb.Tag.ToString() == "mp3" ? Recorder.Format.Mp3 : Recorder.Format.Wav;
        }

        private void tgAddFolders_CheckedChanged(object sender, EventArgs e)
        {
            _strucDossiers = tgAddFolders.Checked;
        }

        private void tgAddSeparators_CheckedChanged(object sender, EventArgs e)
        {
            _charSeparator = tgAddSeparators.Checked ? "_" : " ";
        }

        private void tgNumFiles_CheckedChanged(object sender, EventArgs e)
        {
            _bNumFile = tgNumFiles.Checked;
        }

        private void frmEspionSpotify_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Watcher.Ready) return;
            e.Cancel = true;
            MessageBox.Show(
                @"Veuillez arrêter l'écoute avant de quitter l'espion.",
                @"Écoute en cours",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void tgNumTracks_CheckedChanged(object sender, EventArgs e)
        {
            _bCdTrack = tgNumTracks.Checked;
        }

        private void lblNum_TextChanged(object sender, EventArgs e)
        {
            var val = Convert.ToInt16(lblNum.Text);
            _num = val;
        }

        private void lnkPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            txtPath.Text = folderBrowserDialog.SelectedPath;
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
    }
}
