using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using NAudio.Lame;

namespace EspionSpotify
{
    public partial class FrmEspionSpotify : Form
    {
        private readonly VolumeWin _sound;
        private Watcher _watcher;
        private int _minTime;
        private bool _strucDossiers;
        private Recorder.Format _formatValue;
        private string _charSeparator;
        private bool _bCdTrack;
        private bool _bNumFile;
        private int _cdNumTrack;

        public FrmEspionSpotify()
        {
            InitializeComponent();
            _minTime = 30;
            _formatValue = Recorder.Format.Mp3;
            _charSeparator = "_";
            _bCdTrack = false;
            _bNumFile = false;
            _cdNumTrack = 1;
            txtPath.Text = Properties.Settings.Default.Directory;
            folderBrowserDialog.SelectedPath = Properties.Settings.Default.Directory;

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

            cbBitRate.SelectedIndex = Properties.Settings.Default.Bitrate = 1;
            
            _sound = new VolumeWin();

            tbVolumeWin.Value = _sound.DefaultAudioDeviceVolume/5;
            lblSoundCard.Text = _sound.DefaultAudioEndPointDevice.FriendlyName;
            lblVolume.Text = _sound.DefaultAudioDeviceVolume + @"%";
        }

        public delegate void PrintStatusLineDelegate(string text);

        public delegate void PrintCurrentlyPlayingDelegate(string text);

        public void PrintCurrentlyPlaying(string text)
        {
            if (currentlyPlayingLabel.InvokeRequired)
            {
                var del = new PrintCurrentlyPlayingDelegate(PrintCurrentlyPlaying);    
                currentlyPlayingLabel.Invoke(del, text);
            }
            else
            {
                currentlyPlayingLabel.Text = text;
            }
        }
      
        public void PrintStatusLine(string text)
        {
            if(rtbLog.InvokeRequired)
            {
                var del = new PrintStatusLineDelegate(PrintStatusLine);
                rtbLog.Invoke(del, text);
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
                    rtbLog.SelectionColor = Color.DeepSkyBlue;
                    rtbLog.AppendText(msg + Environment.NewLine);
                    rtbLog.Select(rtbLog.TextLength - msg.Length, msg.Length);
                    rtbLog.SelectionColor = commercial ? Color.White : Color.Tomato;
                }
                else
                {
                    rtbLog.AppendText(text + Environment.NewLine);
                    rtbLog.Select(rtbLog.TextLength - text.Length - 1, text.Length);
                    rtbLog.SelectionColor = Color.White;
                }

                rtbLog.SelectionStart = rtbLog.TextLength;
                rtbLog.ScrollToCaret();
            }
        }

        private void StartRecording()
        {
            Properties.Settings.Default.Directory = folderBrowserDialog.SelectedPath;
            Properties.Settings.Default.Bitrate = cbBitRate.SelectedIndex;
            Properties.Settings.Default.Save();

            var bitrate = ((KeyValuePair<LAMEPreset, string>)cbBitRate.SelectedItem).Key;
            var format = _formatValue;

            _watcher = new Watcher(this, txtPath.Text, bitrate, format, _sound, _minTime, 
                _strucDossiers, _charSeparator, _bCdTrack, _bNumFile, _cdNumTrack);

            var watcherThread = new Thread(_watcher.Run);
            watcherThread.Start();
 
            recordButton.Text = @"Arrêter l'écoute";
            txtPath.Enabled = false;
            btnPath.Enabled = false;
            chkAddFolders.Enabled = false;
            chkAddSeparator.Enabled = false;
            numTrack.Enabled = false;
            chkNumFile.Enabled = false;
            chkCdNums.Enabled = false;
            rbMp3.Enabled = false;
            rbWav.Enabled = false;
            cbBitRate.Enabled = false;
            tbMinTime.Enabled = false;
            timer1.Start();
        }

        public void StopRecording()
        {
            Watcher.Running = false;            
            recordButton.Text = @"Débuter l'écoute";
            txtPath.Enabled = true;
            btnPath.Enabled = true;
            chkAddFolders.Enabled = true;
            chkAddSeparator.Enabled = true;
            numTrack.Enabled = true;
            chkNumFile.Enabled = true;
            chkCdNums.Enabled = true;
            rbMp3.Enabled = true;
            rbWav.Enabled = true;
            cbBitRate.Enabled = true;
            tbMinTime.Enabled = true;
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

        private void directoryButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            txtPath.Text = folderBrowserDialog.SelectedPath;
        }

        private void recordButton_Click(object sender, EventArgs e)
        {
            if(!Watcher.Running)
            {
                if(DirExists()) StartRecording();
            }
            else
            {
                StopRecording();
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            rtbLog.Text = "";
        }

        private void dirButton_Click(object sender, EventArgs e)
        {
            if (DirExists()) System.Diagnostics.Process.Start("explorer.exe", txtPath.Text);
        }

        private void tbVolumeWin_Scroll(object sender, EventArgs e)
        {
            if (_sound.DefaultAudioEndPointDevice.AudioEndpointVolume.Mute)
                _sound.DefaultAudioEndPointDevice.AudioEndpointVolume.Mute=false;

            _sound.SetDefaultAudioDeviceVolume(tbVolumeWin.Value*5);
            lblVolume.Text = (tbVolumeWin.Value*5) + @"%";
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

        private void tbMinTime_Scroll(object sender, EventArgs e)
        {
            _minTime = tbMinTime.Value * 10;
            var min = (_minTime / 60);
            var sec = (_minTime % 60);
            lblMinTime.Text = min + @":" + sec.ToString("00");
        }

        private void cbAddFolders_CheckedChanged(object sender, EventArgs e)
        {
            _strucDossiers = chkAddFolders.Checked;
        }

        private void cbAddSeparator_CheckedChanged(object sender, EventArgs e)
        {
            _charSeparator = chkAddSeparator.Checked ? "_" : " ";
        }

        private void chkCdNums_CheckedChanged(object sender, EventArgs e)
        {
            _bCdTrack = chkCdNums.Checked;
        }

        private void chkNumFile_CheckedChanged(object sender, EventArgs e)
        {
            _bNumFile = chkNumFile.Checked;
        }

        public void UpdateCdTrackNum(int num)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateCdTrackNum(num)));
            }
            else
            {
                numTrack.Value = num;
            }
        }

        private void numTrack_ValueChanged(object sender, EventArgs e)
        {
            _cdNumTrack = (int)numTrack.Value;
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
    }
}
