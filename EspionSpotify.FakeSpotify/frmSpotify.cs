using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace EspionSpotify.FakeSpotify
{
    public sealed partial class FrmSpotify : Form
    {
        private int _lastPlayedIndice;
        private bool _isPlaying;
        private WaveOut _waveOut;
        
        public FrmSpotify()
        {
            InitializeComponent();
            _waveOut = CreateWave();
        }

        private void tbDelayTitle_Scroll(object sender, ScrollEventArgs e)
        {
            this.lblDelayTitle.Text = e.NewValue.ToString() + "ms";
        }

        private void btnAddToPlaylist_Click(object sender, EventArgs e)
        {
            this.lstPlaylist.Items.Add(txtAddToPlaylist.Text);
            txtAddToPlaylist.Clear();
        }

        private void btnDeleteInPlaylist_Click(object sender, EventArgs e)
        {
            var items = this.lstPlaylist.SelectedItems.Cast<ListViewItem>();
            foreach (var item in items)
            {
                this.lstPlaylist.Items.Remove(item);
            }
        }
        
        private void lstPlaylist_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.btnDeleteInPlaylist.Enabled = e.IsSelected;
        }

        private void btnNextTrack_Click(object sender, EventArgs e)
        {
            _lastPlayedIndice += 1;
            if (_lastPlayedIndice >= this.lstPlaylist.Items.Count)
            {
                _lastPlayedIndice = 0;
            }

            ChangeTitle(fromList: true);
            
            if (chkLockWindowTitleToPlaybackState.Checked)
            {
                ValidPlayback();
            }
        }

        private void ChangeTitle(bool fromList)
        {
            var title = fromList ? this.lstPlaylist.Items[_lastPlayedIndice].Text: this.txtWindowTitle.Text;
            this.txtWindowTitle.Text = title;
            
            ChangeWindowTitle(title);
        }

        private void ChangeWindowTitle(string title)
        {
            var t = new Thread(() =>
            {
                Thread.Sleep(tbDelayTitle.Value);
                SetFormPropertyThreadSafe(() => this.Text = title);
            });
            t.Start();
        }

        private void ValidPlayback()
        {
            if (IsWindowTitledSpotify && _waveOut.PlaybackState != PlaybackState.Stopped)
            {
                _waveOut.Stop();
                _isPlaying = false;
                this.btnPlayback.Text = "Play";
            }
            else if (_waveOut.PlaybackState != PlaybackState.Playing)
            {
                _waveOut.Play();
                _isPlaying = true;
                this.btnPlayback.Text = "Pause";
            }
        }

        private void btnPlayback_Click(object sender, EventArgs e)
        {
            _isPlaying = !_isPlaying;
            if (chkLockWindowTitleToPlaybackState.Enabled && !_isPlaying)
            {
                this.txtWindowTitle.Text = "Spotify";
            }
            ChangeTitle(fromList: false);
            ValidPlayback();
        }

        private void txtWindowTitle_Leave(object sender, EventArgs e)
        {
            _isPlaying = !IsWindowTitledSpotify;

            ChangeWindowTitle(this.txtWindowTitle.Text);
        }

        private WaveOut CreateWave()
        {
            var signalGenerator = new SignalGenerator();
            signalGenerator.Type = SignalGeneratorType.Sweep;
            signalGenerator.Frequency = 50;
            signalGenerator.FrequencyEnd = 5000;
            signalGenerator.SweepLengthSecs = 3;

            var waveOut = new WaveOut();
            waveOut.DeviceNumber = 0;
            waveOut.Init(signalGenerator);

            return waveOut;
        }

        private void SetFormPropertyThreadSafe(MethodInvoker setter)
        {
            lock (this)
            {
                if (this.IsDisposed) return;
                if (this.InvokeRequired)
                    this.Invoke(setter);
                else
                    setter();
            }
        }

        private void chkLockWindowTitleToPlaybackState_CheckedChanged(object sender, EventArgs e)
        {
            if ((_isPlaying && IsWindowTitledSpotify) || (!_isPlaying && !IsWindowTitledSpotify))
            {
                ValidPlayback();
            }
        }

        private bool IsWindowTitledSpotify => this.txtWindowTitle.Text.Trim().ToLower() == "spotify";

        private void tbVolume_Scroll(object sender, ScrollEventArgs e)
        {
            _waveOut.Volume = this.tbVolume.Value/ 100.0f;
        }

        private void lstPlaylist_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _lastPlayedIndice = this.lstPlaylist.SelectedIndices[0];
            ChangeTitle(fromList: true);
            
            if (chkLockWindowTitleToPlaybackState.Checked)
            {
                ValidPlayback();
            }
        }
    }
}