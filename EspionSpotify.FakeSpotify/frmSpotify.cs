using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private object _lockObject = new object();
        
        public FrmSpotify()
        {
            InitializeComponent();
            _waveOut = CreateWave();

            _waveOut.Volume = 0.0f;
            this.tbVolume.Value = 0;
        }

        private void tbDelay_Scroll(object sender, ScrollEventArgs e)
        {
            this.lblDelay.Text = e.NewValue.ToString() + "ms";
        }

        private void tbSilence_Scroll(object sender, ScrollEventArgs e)
        {
            this.lblSilence.Text = e.NewValue.ToString() + "ms";
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
            NextTrack();
        }

        private void NextTrack()
        {
            var silenceMs = 0;
            var silenceVolume = 0.0f;
            AccessFormPropertyThreadSafe(() =>
            {
                silenceMs = tbSilence.Value;
                silenceVolume = tbSilenceVolume.Value / 100.0f;
            });

            var t = new Task(() =>
            {
                _lastPlayedIndice += 1;
                if (_lastPlayedIndice >= this.lstPlaylist.Items.Count)
                {
                    _lastPlayedIndice = 0;
                }

                ChangeTitle(fromList: true);

                if (chkLockWindowTitleToPlaybackState.Checked)
                {
                    var fromVolume = _waveOut.Volume;
                    var toVolume = fromVolume * silenceVolume;

                    while (_waveOut.Volume > toVolume)
                    {
                        _waveOut.Volume = Math.Max(0, _waveOut.Volume / 2);
                        Thread.Sleep(1);
                    }

                    _waveOut.Volume = toVolume;

                    Thread.Sleep(silenceMs);

                    AccessFormPropertyThreadSafe(() =>
                    {
                        var prev = _waveOut;
                        _waveOut = CreateWave();
                        prev.Dispose();
                    });

                    ValidPlayback();

                    if (toVolume == 0f)
                    {
                        // to be able to run while loop
                        _waveOut.Volume = 0.0001f;
                    }

                    while (_waveOut.Volume < fromVolume)
                    {
                        _waveOut.Volume = Math.Min(_waveOut.Volume * 2, 1);
                        Thread.Sleep(1);
                    }

                    _waveOut.Volume = fromVolume;
                }
            });

            t.Start();
        }

        private void ChangeTitle(bool fromList)
        {
            var title = "";
          
            AccessFormPropertyThreadSafe(() =>
            {
                title = fromList ? this.lstPlaylist.Items[_lastPlayedIndice].Text : this.txtWindowTitle.Text;
                this.txtWindowTitle.Text = title;
            });

            ChangeWindowTitle(title);
        }

        private void ChangeWindowTitle(string title)
        {
            var t = new Thread(() =>
            {
                Thread.Sleep(tbDelay.Value);
                AccessFormPropertyThreadSafe(() => this.Text = title);
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
            
            if (chkLockWindowTitleToPlaybackState.Checked)
            {
                ValidPlayback();
            }
        }

        private WaveOut CreateWave()
        {
            var rand = new Random();
            var type = rand.Next(0, 6);
            var signalGenerator = new SignalGenerator();
            signalGenerator.Type =  (SignalGeneratorType) type;
            signalGenerator.Frequency = 400;
            signalGenerator.FrequencyEnd = 500;
            signalGenerator.SweepLengthSecs = 2;

            var waveOut = new WaveOut();
            waveOut.DeviceNumber = 0;
            waveOut.Init(signalGenerator);

            return waveOut;
        }

        private void AccessFormPropertyThreadSafe(MethodInvoker setter)
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
            _waveOut.Volume = e.NewValue / 100.0f;
            lblVolume.Text = e.NewValue.ToString() + "%";
        }

        private void lstPlaylist_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_lastPlayedIndice != this.lstPlaylist.SelectedIndices[0])
            {
                //NextTrack();
            }

            _lastPlayedIndice = this.lstPlaylist.SelectedIndices[0];
            ChangeTitle(fromList: true);
            
            if (chkLockWindowTitleToPlaybackState.Checked)
            {
                ValidPlayback();
            }
        }

        private void tbSilenceVolume_Scroll(object sender, ScrollEventArgs e)
        {
            lblSilenceVolume.Text = e.NewValue.ToString() + "%";
        }
    }
}