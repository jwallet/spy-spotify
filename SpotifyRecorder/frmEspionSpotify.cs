using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using NAudio.Lame;

namespace EspionSpotify
{
    public partial class frmEspionSpotify : Form
    {
        VolumeWin Sound;
        Watcher watcher;
        private int minTime;
        private bool strucDossiers;
        Recorder.Format formatValue;
        private string charSeparator;
        public frmEspionSpotify()
        {
            
            InitializeComponent();
            minTime = 30;
            formatValue = Recorder.Format.mp3;
            charSeparator = "_";
            directoryTextBox.Text = EspionSpotify.Properties.Settings.Default.Directory;
            folderBrowserDialog.SelectedPath = EspionSpotify.Properties.Settings.Default.Directory;

            Dictionary<LAMEPreset, string> bitrates = new Dictionary<LAMEPreset, string>
            {
                {LAMEPreset.ABR_128, "128kbps"},
                {LAMEPreset.ABR_160, "160kbps (Spotify Free)"},
                {LAMEPreset.ABR_256, "256kbps"},
                {LAMEPreset.ABR_320, "320kbps Haute qualité (Spotify Premium)"}
            };

            bitrateComboBox.DataSource = new BindingSource(bitrates, null);
            bitrateComboBox.DisplayMember = "Value";
            bitrateComboBox.ValueMember = "Key";

            bitrateComboBox.SelectedIndex = EspionSpotify.Properties.Settings.Default.Bitrate = 1;
            
            Sound = new VolumeWin();

            int Volume = Sound.DefaultAudioDeviceVolume;
            tbVolumeWin.Value = Sound.DefaultAudioDeviceVolume/5;
            lblSoundCard.Text = Sound.DefaultAudioEndPointDevice.FriendlyName;
            lblVolume.Text = Sound.DefaultAudioDeviceVolume.ToString() + "%";
        }

        delegate void PrintStatusLineDelegate(string text);
        delegate void PrintCurrentlyPlayingDelegate(string text);

        public void PrintCurrentlyPlaying(string text)
        {
            if (currentlyPlayingLabel.InvokeRequired)
            {
                PrintCurrentlyPlayingDelegate del = new PrintCurrentlyPlayingDelegate(PrintCurrentlyPlaying);    
                currentlyPlayingLabel.Invoke(del, text);
            }
            else
            {
                currentlyPlayingLabel.Text = text;
            }
        }
      
        public void PrintStatusLine(string text)
        {
            if(statusTextBox.InvokeRequired)
            {
                PrintStatusLineDelegate del = new PrintStatusLineDelegate(PrintStatusLine);
                statusTextBox.Invoke(del, text);
            }
            else
            {
                string timeStr = DateTime.Now.ToString("HH:mm:ss");
                statusTextBox.Text = statusTextBox.Text + String.Format("[{0}] {1}", timeStr, text) + Environment.NewLine;
                statusTextBox.SelectionStart = statusTextBox.TextLength;
                statusTextBox.ScrollToCaret();
            }
        }

        private void startRecording()
        {
            EspionSpotify.Properties.Settings.Default.Directory = folderBrowserDialog.SelectedPath;
            EspionSpotify.Properties.Settings.Default.Bitrate = bitrateComboBox.SelectedIndex;
            EspionSpotify.Properties.Settings.Default.Save();

            LAMEPreset bitrate = ((KeyValuePair<LAMEPreset, string>)bitrateComboBox.SelectedItem).Key;
            Recorder.Format format = formatValue;
            watcher = new Watcher(this, directoryTextBox.Text, bitrate, format, Sound, minTime, strucDossiers, charSeparator);
            Thread watcherThread = new Thread(watcher.Run);
            watcherThread.Start();
 
            recordButton.Text = "Arrêter l\'écoute";
            directoryTextBox.Enabled = false;
            directoryButton.Enabled = false;
            bitrateComboBox.Enabled = false;
            tbMinTime.Enabled = false;
            rbMp3.Enabled = false;
            rbWav.Enabled = false;
            cbAddFolders.Enabled = false;
            cbAddSeparator.Enabled = false;
            timer1.Start();
        }

        public void stopRecording()
        {
            Watcher.Running = false;            
            recordButton.Text = "Débuter l\'écoute";
            directoryTextBox.Enabled = true;
            directoryButton.Enabled = true;
            bitrateComboBox.Enabled = true;
            tbMinTime.Enabled = true;
            rbMp3.Enabled = true;
            rbWav.Enabled = true;
            cbAddFolders.Enabled = true;
            cbAddSeparator.Enabled = true;
            timer1.Stop();
        }

        private bool DirExists()
        {
            if (!Directory.Exists(directoryTextBox.Text))
            {
                MessageBox.Show(
                    "Le répertoire de sauvegarde sélectionné n\'existe pas.",
                    "Répertoire invalide",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void directoryButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            directoryTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void recordButton_Click(object sender, EventArgs e)
        {
            if(!Watcher.Running)
            {
                if(DirExists())
                {
                    startRecording();
                }
            }
            else
            {
                stopRecording();
            }
        }

        private void SpotifyRecorderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!Watcher.Ready)
            {
                e.Cancel = true;
                MessageBox.Show(
                        "Veuillez arrêter l\'écoute avant de quitter l\'espion.",
                        "Écoute en cours",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            statusTextBox.Text = "";
        }

        private void dirButton_Click(object sender, EventArgs e)
        {
            if (DirExists())
            {
                System.Diagnostics.Process.Start("explorer.exe", directoryTextBox.Text);
            }
        }

        private void tbVolumeWin_Scroll(object sender, EventArgs e)
        {
            if (Sound.DefaultAudioEndPointDevice.AudioEndpointVolume.Mute==true)
                Sound.DefaultAudioEndPointDevice.AudioEndpointVolume.Mute=false;
            Sound.SetDefaultAudioDeviceVolume(tbVolumeWin.Value*5);
            lblVolume.Text = (tbVolumeWin.Value*5).ToString() + "%";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            watcher.count++;
            if(!Watcher.Running)
            {
                stopRecording();
            }
        }

        private void rbsec_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
            {
                minTime = int.Parse(rb.Tag.ToString());
            }
        }

        private void rbFormat_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Tag.ToString() == "mp3")
            {
                formatValue = Recorder.Format.mp3;
            }
            else
                formatValue = Recorder.Format.wav;
        }

        private void tbMinTime_Scroll(object sender, EventArgs e)
        {
            int min;
            int sec;
            minTime = tbMinTime.Value * 10;
            min = (int)(minTime / 60);
            sec = (minTime % 60);
            lblMinTime.Text = min + ":" + sec.ToString("00");
        }

        private void cbAddFolders_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAddFolders.Checked)
                strucDossiers = true;
            else
                strucDossiers = false;
        }

        private void cbAddSeparator_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAddSeparator.Checked)
                charSeparator = "_";
            else
                charSeparator = " ";
        }

    }
}
