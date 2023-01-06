using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;
using EspionSpotify.API;
using EspionSpotify.AudioSessions;
using EspionSpotify.Controls;
using EspionSpotify.Drivers;
using EspionSpotify.Enums;
using EspionSpotify.Extensions;
using EspionSpotify.Models;
using EspionSpotify.Native;
using EspionSpotify.Properties;
using EspionSpotify.Translations;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;
using NAudio.Lame;

namespace EspionSpotify
{
    public sealed partial class FrmEspionSpotify : MetroForm, IFrmEspionSpotify
    {
        private readonly Analytics _analytics;
        private readonly IMainAudioSession _audioSession;

        private readonly TranslationKeys[] _recorderStatusTranslationKeys =
        {
            I18NKeys.LogRecording,
            I18NKeys.LogRecorded,
            I18NKeys.LogDeleting,
            I18NKeys.LogTrackExists
        };
        private readonly UserSettings _userSettings;
        private FrmSpotifyAPICredentials _frmSpotifyApiCredentials;
        private bool _toggleStopRecordingDelayed;
        private Watcher _watcher;
        private bool _watching;

        public FrmEspionSpotify()
        {
            InitializeComponent();
            SuspendLayout();

            Text = Constants.SPYTIFY;

            _audioSession = new MainAudioSession(Settings.Default.app_selected_audio_device_id);
            _userSettings = new UserSettings();

            Updater.UpgradeSettings();

            if (string.IsNullOrEmpty(Settings.Default.settings_output_path))
            {
                Settings.Default.settings_output_path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                Settings.Default.Save();
            }

            if (string.IsNullOrEmpty(Settings.Default.app_analytics_cid))
            {
                Settings.Default.app_analytics_cid = Analytics.GenerateCid();
                Settings.Default.Save();
            }

            _analytics = new Analytics(Settings.Default.app_analytics_cid,
                Assembly.GetExecutingAssembly().GetName().Version.ToString());

            Instance = this;
            ResumeLayout();

            Init();

            Task.Run(async () =>
            {
                await _analytics.LogAction("launch");
                await GitHub.GetVersion();
            });
        }

        public static FrmEspionSpotify Instance { get; private set; }

        private string LogDate => $@"[{DateTime.Now:HH:mm:ss}] ";

        public ResourceManager Rm { get; private set; }

        public void UpdateNumDown()
        {
            if (!_userSettings.HasOrderNumberEnabled) return;

            _userSettings.InternalOrderNumber--;
            UpdateNum(_userSettings.InternalOrderNumber);
        }

        public void UpdateNumUp()
        {
            if (!_userSettings.HasOrderNumberEnabled) return;

            _userSettings.InternalOrderNumber++;
            UpdateNum(_userSettings.InternalOrderNumber);
        }

        public void UpdateStartButton()
        {
            lnkSpy.SetPropertyThreadSafe(() =>
            {
                tip.SetToolTip(lnkSpy, Rm.GetString(I18NKeys.TipStartSpying));
                lnkSpy.Image = Resources.on;
                lnkSpy.Focus();
            });
        }

        public void UpdateIconSpotify(bool isSpotifyPlaying, bool isRecording = false)
        {
            iconSpotify.SetPropertyThreadSafe(() =>
            {
                if (isRecording)
                {
                    iconSpotify.BackgroundImage = Resources.record;
                    lblPlayingTitle.ForeColor = lblPlayingTitle.ForeColor.SpotifyPrimaryText();
                    Task.Run(async () => await _analytics.LogAction("record"));
                }
                else if (isSpotifyPlaying)
                {
                    iconSpotify.BackgroundImage = Resources.play;
                    lblPlayingTitle.ForeColor = lblPlayingTitle.ForeColor.SpotifySecondaryText();
                    Task.Run(async () => await _analytics.LogAction("play"));
                }
                else
                {
                    iconSpotify.BackgroundImage = Resources.pause;
                    lblPlayingTitle.ForeColor = lblPlayingTitle.ForeColor.SpotifySecondaryText();
                    Task.Run(async () => await _analytics.LogAction("pause"));
                }
            });
        }

        public void UpdatePlayingTitle(string text)
        {
            lblPlayingTitle.SetPropertyThreadSafe(() => lblPlayingTitle.Text = text);
        }

        public void UpdateRecordedTime(int? time)
        {
            lblRecordedTime.SetPropertyThreadSafe(() =>
                lblRecordedTime.Text = time.HasValue ? TimeSpan.FromSeconds(time.Value).ToString(@"mm\:ss") : "");
        }

        public void WriteIntoConsole(TranslationKeys resource, params object[] args)
        {
            rtbLog.SetPropertyThreadSafe(() =>
            {
                var log = WriteRtbLine(rtbLog, resource, args);

                if (!string.IsNullOrEmpty(log))
                {
                    Settings.Default.app_console_logs += $";{log}";
                    Settings.Default.Save();
                }
            });
        }

        public void StopRecording()
        {
            if (tlSettings.InvokeRequired || tlAdvanced.InvokeRequired)
            {
                BeginInvoke(new Action(StopRecording));
                return;
            }

            Watcher.Running = false;
            ExternalAPI.Instance.Reset();
            _toggleStopRecordingDelayed = false;
            timer1.Stop();

            if (tlSettings.IsDisposed() || tlAdvanced.IsDisposed()) return;

            tlSettings.Enabled = true;
            tlAdvanced.Enabled = true;
            _watching = false;
        }

        public void SetSoundVolume(int volume)
        {
            tbVolumeWin.SetPropertyThreadSafe(() => tbVolumeWin.Value = volume);
        }

        private void Init()
        {
            tcMenu.SelectedIndex = Settings.Default.app_tab_number_selected;

            rbMp3.Checked = Settings.Default.settings_media_audio_format == (int) MediaFormat.Mp3;
            rbWav.Checked = Settings.Default.settings_media_audio_format == (int) MediaFormat.Wav;
            tbMinTime.Value = Settings.Default.settings_media_minimum_recorded_length_in_seconds / 5;
            tgListenToSpotifyPlayback.Checked = Settings.Default.advanced_watcher_listen_to_spotify_playback_enabled;
            tgAddSeparators.Checked = Settings.Default.advanced_file_replace_space_by_underscore_enabled;
            tgCounterToMediaTag.Checked = Settings.Default.advanced_id3_counter_number_as_track_number_enabled;
            tgCounterToFilePrefix.Checked = Settings.Default.advanced_file_counter_number_prefix_enabled;
            tgAddFolders.Checked = Settings.Default.advanced_file_group_media_in_folders_enabled;
            txtPath.Text = Settings.Default.settings_output_path;
            tgMuteAds.Checked = Settings.Default.settings_mute_ads_enabled;
            tgMinimizeToSystemTray.Checked = Settings.Default.settings_minimize_to_system_tray_enabled;
            tgExtraTitleToSubtitle.Checked = Settings.Default.advanced_id3_extra_title_as_subtitle_enabled;
            folderBrowserDialog.SelectedPath = Settings.Default.settings_output_path;
            txtRecordingNum.Mask = Settings.Default.app_counter_number_mask;

            tgRecordOverRecordings.Checked = Settings.Default.advanced_record_over_recordings_enabled;
            chkRecordDuplicateRecordings.Enabled = Settings.Default.advanced_record_over_recordings_enabled;
            chkRecordDuplicateRecordings.Checked =
                Settings.Default.advanced_record_over_recordings_and_duplicate_enabled;
            chkRecordDuplicateRecordings.Visible = Settings.Default.advanced_record_over_recordings_enabled;

            tgRecordEverything.Checked = Settings.Default.advanced_record_everything;
            tgForceSpotifyToSkip.Checked = Settings.Default.advanced_watcher_force_spotify_to_skip;
            chkRecordAds.Enabled = Settings.Default.advanced_record_everything;
            chkRecordAds.Checked = Settings.Default.advanced_record_everything_and_ads_enabled;
            chkRecordAds.Visible = Settings.Default.advanced_record_everything;

            SetSpotifyAPIOption();

            rbLastFMAPI.Checked = Settings.Default.app_selected_external_api_id == (int) ExternalAPIType.LastFM ||
                                  !_userSettings.IsSpotifyAPISet;
            rbSpotifyAPI.Checked = Settings.Default.app_selected_external_api_id == (int) ExternalAPIType.Spotify &&
                                   _userSettings.IsSpotifyAPISet;

            ReloadExternalAPI();

#if DEBUG
            Style = MetroColorStyle.Orange;
#endif

            SetLanguageDropDown(); // do it before setting the language
            SetLanguage(); // creates Rm and trigger fields event which requires audioSession

            UpdateAudioEndPointFields(_audioSession.AudioDeviceVolume,
                _audioSession.AudioMMDevicesManager.AudioEndPointDeviceName);
            SetAudioEndPointDevicesDropDown(); // affects data source which requires Rm and audioSession
            UpdateAudioVirtualCableDriverImage();

            _userSettings.AudioEndPointDeviceID = _audioSession.AudioMMDevicesManager.AudioEndPointDeviceID;
            _userSettings.Bitrate = cbBitRate.SelectedItem.ToKeyValuePair<LAMEPreset, string>().Key;
            _userSettings.RecordRecordingsStatus = Settings.Default.GetRecordRecordingsStatus();
            _userSettings.ListenToSpotifyPlaybackEnabled =
                Settings.Default.advanced_watcher_listen_to_spotify_playback_enabled;
            _userSettings.GroupByFoldersEnabled = Settings.Default.advanced_file_group_media_in_folders_enabled;
            _userSettings.MediaFormat = (MediaFormat) Settings.Default.settings_media_audio_format;
            _userSettings.MinimumRecordedLengthSeconds =
                Settings.Default.settings_media_minimum_recorded_length_in_seconds;
            _userSettings.OrderNumberInfrontOfFileEnabled =
                Settings.Default.advanced_file_counter_number_prefix_enabled;
            _userSettings.AlbumTrackNumberInfrontOfFileEnabled =
                Settings.Default.advanced_file_album_track_number_prefix_enabled;
            _userSettings.OrderNumberInMediaTagEnabled =
                Settings.Default.advanced_id3_counter_number_as_track_number_enabled;
            _userSettings.OutputPath = FileManager.GetCleanPath(Settings.Default.settings_output_path);
            _userSettings.ForceSpotifyToSkipEnabled = Settings.Default.advanced_watcher_force_spotify_to_skip;
            _userSettings.RecordEverythingEnabled = Settings.Default.advanced_record_everything;
            _userSettings.RecordAdsEnabled = Settings.Default.advanced_record_everything_and_ads_enabled;
            _userSettings.MuteAdsEnabled = Settings.Default.settings_mute_ads_enabled;
            _userSettings.MinimizeToSystemTrayEnabled = Settings.Default.settings_minimize_to_system_tray_enabled;
            _userSettings.TrackTitleSeparator =
                Settings.Default.advanced_file_replace_space_by_underscore_enabled ? "_" : " ";
            _userSettings.OrderNumberMask = Settings.Default.app_counter_number_mask;
            _userSettings.ExtraTitleToSubtitleEnabled = Settings.Default.advanced_id3_extra_title_as_subtitle_enabled;

            txtRecordingNum.Text = _userSettings.InternalOrderNumber.ToString(_userSettings.OrderNumberMask);

            var logs = Settings.Default.app_console_logs.Split(';').Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
            WritePreviousLogsIntoConsole(logs);

            var lastVersionPrompted = Settings.Default.app_last_version_prompt.ToVersion();
            lnkRelease.Visible = lastVersionPrompted != null &&
                                 lastVersionPrompted > Assembly.GetExecutingAssembly().GetName().Version;
        }

        private void SetSpotifyAPIOption()
        {
            _userSettings.SpotifyAPIClientId = Settings.Default.app_spotify_api_client_id?.Trim();
            _userSettings.SpotifyAPISecretId = Settings.Default.app_spotify_api_client_secret?.Trim();
            _userSettings.SpotifyAPIRedirectURL = Settings.Default.app_spotify_api_redirect_url?.Trim();
            rbSpotifyAPI.Enabled = _userSettings.IsSpotifyAPISet;
        }

        private void ReloadExternalAPI()
        {
            if (Settings.Default.settings_media_audio_format == (int) MediaFormat.Wav)
            {
                SetExternalAPI(ExternalAPIType.None);
                tlpAPI.Visible = false;
                lblAPI.Visible = false;
                return;
            }

            tlpAPI.Visible = true;
            lblAPI.Visible = true;
            if (_userSettings.IsSpotifyAPISet &&
                Settings.Default.app_selected_external_api_id == (int) ExternalAPIType.Spotify)
            {
                SetExternalAPI(ExternalAPIType.Spotify, _userSettings.IsSpotifyAPISet);
                return;
            }
            
            SetExternalAPI(ExternalAPIType.LastFM);
        }

        private void SetExternalAPI(ExternalAPIType api, bool isSpotifyAPISet = false)
        {
            switch (api)
            {
                case ExternalAPIType.Spotify:
                    if (isSpotifyAPISet)
                        ExternalAPI.Instance = new API.SpotifyAPI(
                            _userSettings.SpotifyAPIClientId,
                            _userSettings.SpotifyAPISecretId,
                            _userSettings.SpotifyAPIRedirectURL);
                    break;
                case ExternalAPIType.LastFM:
                    ExternalAPI.Instance = new LastFMAPI();
                    break;
                default:
                    ExternalAPI.Instance = new NoneAPI();
                    break;
            }
        }

        private void UpdateAudioEndPointFields(int volume, string friendlyName)
        {
            var isSet = friendlyName != null;

            tbVolumeWin.Enabled = isSet;
            tbVolumeWin.Visible = isSet;
            iconVolume.Visible = isSet;
            lblVolume.Visible = isSet;

            lblSoundCard.Text = friendlyName;
            lblVolume.Text = volume + @"%";
            tbVolumeWin.Value = volume;
        }

        public void SetAudioEndPointDevicesDropDown()
        {
            var selectedID = _audioSession.IsAudioEndPointDeviceIndexAvailable
                ? _audioSession.AudioMMDevicesManager.AudioEndPointDeviceID
                : _audioSession.AudioMMDevicesManager.DefaultAudioEndPointDeviceID;

            var isAudioDeviceEmpty = _audioSession.AudioMMDevicesManager.AudioEndPointDeviceNames.Count == 0;

            cbAudioDevices.Enabled = _audioSession.AudioMMDevicesManager.AudioEndPointDeviceNames.Count > 1;

            if (isAudioDeviceEmpty)
            {
                cbAudioDevices.DataSource = null;
                cbAudioDevices.SelectedIndex = -1;
            }
            else
            {
                cbAudioDevices.DataSource = new BindingSource(
                    _audioSession.AudioMMDevicesManager.AudioEndPointDeviceNames,
                    null);
                cbAudioDevices.DisplayMember = "Value";
                cbAudioDevices.ValueMember = "Key";
                cbAudioDevices.SelectedValue = selectedID;
            }
        }

        private void SetLanguageDropDown()
        {
            var value = Settings.Default.settings_language;

            cbLanguage.DataSource = new BindingSource(Languages.DropdownListValues, null);
            cbLanguage.DisplayMember = "Value";
            cbLanguage.ValueMember = "Key";
            cbLanguage.SelectedItem = Languages.GetDropdownListItemFromLanguageType(value.ToLanguageType());
        }

        private void SetLanguage()
        {
            var language = Settings.Default.settings_language;
            var languageType = language.ToLanguageType() ?? LanguageType.en;

            var rmLanguage = Languages.GetResourcesManagerLanguageType(languageType);
            Rm = new ResourceManager(rmLanguage ?? typeof(en));

            tabRecord.Text = Rm.GetString(I18NKeys.TabRecord);
            tabSettings.Text = Rm.GetString(I18NKeys.TabSettings);
            tabAdvanced.Text = Rm.GetString(I18NKeys.TabAdvanced);

            folderBrowserDialog.Description = Rm.GetString(I18NKeys.MsgFolderDialog);

            lblPath.Text = Rm.GetString(I18NKeys.LblPath);
            lblAudioDevice.Text = Rm.GetString(I18NKeys.LblAudioDevice);
            lblBitRate.Text = Rm.GetString(I18NKeys.LblBitRate);
            lblFormat.Text = Rm.GetString(I18NKeys.LblFormat);
            lblMinLength.Text = Rm.GetString(I18NKeys.LblMinLength);
            lblLanguage.Text = Rm.GetString(I18NKeys.LblLanguage);
            lblAddFolders.Text = Rm.GetString(I18NKeys.LblAddFolders);
            lblAddSeparators.Text = Rm.GetString(I18NKeys.LblAddSeparators);
            lblCounterToFilePrefix.Text = Rm.GetString(I18NKeys.LblCounterToFilePrefix);
            lblCounterToMediaTag.Text = Rm.GetString(I18NKeys.LblCounterToMediaTag);
            lblAlbumTrackNumberToFilePrefix.Text = Rm.GetString(I18NKeys.LblAlbumTrackNumberToFilePrefix);
            lblListenToSpotifyPlayback.Text = Rm.GetString(I18NKeys.LblListenToSpotifyPlayback);
            lblRecordingNum.Text = Rm.GetString(I18NKeys.LblRecordingNum);
            lblAds.Text = Rm.GetString(I18NKeys.LblAds);
            lblMuteAds.Text = Rm.GetString(I18NKeys.LblMuteAds);
            lblGeneral.Text = Rm.GetString(I18NKeys.LblGeneral);
            lblMinimizeToSystemTray.Text = Rm.GetString(I18NKeys.LblMinimizeToSystemTray);
            lblSpy.Text = Rm.GetString(I18NKeys.LblSpy);
            lblRecorder.Text = Rm.GetString(I18NKeys.LblRecorder);
            lblRecordEverything.Text = Rm.GetString(I18NKeys.LblRecordEverything);
            lblForceSpotifyToSkip.Text = Rm.GetString(I18NKeys.LblForceSpotifyToSkip);
            chkRecordAds.Text = Rm.GetString(I18NKeys.LblRecordAds);
            lblRecordOverRecordings.Text = Rm.GetString(I18NKeys.LblRecordOverRecordings);
            chkRecordDuplicateRecordings.Text = Rm.GetString(I18NKeys.LblDuplicate);
            lblRecordingTimer.Text = Rm.GetString(I18NKeys.LblRecordingTimer);
            lblExtraTitleToSubtitle.Text = Rm.GetString(I18NKeys.LblExtraTitleToSubtitle);
            lblID3.Text = Rm.GetString(I18NKeys.LblID3);
            lblUpdateRecordingsID3Tags.Text = Rm.GetString(I18NKeys.LblUpdateRecordingsID3Tags);

            tip.SetToolTip(lnkClear, Rm.GetString(I18NKeys.TipClear));
            tip.SetToolTip(lnkSpy, Rm.GetString(I18NKeys.TipStartSpying));
            tip.SetToolTip(lnkDirectory, Rm.GetString(I18NKeys.TipDirectory));
            tip.SetToolTip(lnkPath, Rm.GetString(I18NKeys.TipPath));
            tip.SetToolTip(lnkAudioVirtualCable, Rm.GetString(I18NKeys.TipInstallVirtualCableDriver));
            tip.SetToolTip(lnkRelease, Rm.GetString(I18NKeys.TipRelease));
            tip.SetToolTip(lnkDonate, Rm.GetString(I18NKeys.TipDonate));
            tip.SetToolTip(lnkFAQ, Rm.GetString(I18NKeys.TipFAQ));
            tip.SetToolTip(lnkNumPlus, Rm.GetString(I18NKeys.TipNumModifierHold));
            tip.SetToolTip(lnkNumMinus, Rm.GetString(I18NKeys.TipNumModifierHold));
            tip.SetToolTip(lnkSpotifyCredentials, Rm.GetString(I18NKeys.TipSpotifyAPICredentials));

            ReloadBitrateOptions();
        }

        private void ReloadBitrateOptions()
        {
            switch ((MediaFormat)Settings.Default.settings_media_audio_format)
            {
                case MediaFormat.Mp3:
                    SetLamePresetBitrateOptions();
                    cbBitRate.Visible = true;
                    lblBitRate.Visible = true;
                    break;
                default:
                    cbBitRate.Visible = false;
                    lblBitRate.Visible = false;
                    break;
            }
        }

        private void SetLamePresetBitrateOptions()
        {
            var indexBitRate = Settings.Default.settings_media_bitrate_quality;

            var bitrates = new Dictionary<LAMEPreset, string>
            {
                {LAMEPreset.ABR_128, Rm.GetString(I18NKeys.CbOptBitRate128)},
                {
                    LAMEPreset.ABR_160,
                    string.Format(Rm.GetString(I18NKeys.CbOptBitRateSpotifyFree) ?? "{0}",
                        Rm.GetString(I18NKeys.CbOptBitRate160))
                },
                {LAMEPreset.ABR_256, Rm.GetString(I18NKeys.CbOptBitRate256)},
                {
                    LAMEPreset.ABR_320,
                    string.Format(Rm.GetString(I18NKeys.CbOptBitRateSpotifyPremium) ?? "{0}",
                        Rm.GetString(I18NKeys.CbOptBitRate320))
                }
            };

            var isCustomBitrate = indexBitRate > bitrates.Count;
            var isValidPreset = Enum.IsDefined(typeof(LAMEPreset), indexBitRate);

            if (isCustomBitrate && isValidPreset)
            {
                var preset = (LAMEPreset) indexBitRate;
                if (!bitrates.ContainsKey(preset))
                {
                    bitrates.Add(preset, preset.ToString());
                }
            }

            cbBitRate.DataSource = new BindingSource(bitrates, null);
            cbBitRate.DisplayMember = "Value";
            cbBitRate.ValueMember = "Key";

            if (isCustomBitrate)
            {
                cbBitRate.SelectedIndex = bitrates.Count - 1;
            }
            else if (indexBitRate < bitrates.Count)
            {
                cbBitRate.SelectedIndex = indexBitRate;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void UpdateNum(int num)
        {
            txtRecordingNum.SetPropertyThreadSafe(() => txtRecordingNum.Text = num.ToString(txtRecordingNum.Mask));
        }

        public void UpdateExternalAPIToggle(ExternalAPIType value)
        {
            rbLastFMAPI.SetPropertyThreadSafe(() => rbLastFMAPI.Checked = ExternalAPIType.LastFM == value);
        }

        public void ShowFailedToUseSpotifyAPIMessage()
        {
            MetroMessageBox.Show(Instance,
                Rm.GetString(I18NKeys.MsgBodyFailedToUseSpotifyAPI),
                Rm.GetString(I18NKeys.MsgTitleFailedToUseSpotifyAPI),
                MessageBoxButtons.OK,
                MessageBoxIcon.Question);
        }

        private string WriteRtbLine(RichTextBox rtbLog, TranslationKeys resource, params object[] args)
        {
            var text = string.Format(Rm.GetString(resource), args);

            if (text == null || rtbLog.IsDisposed) return "";

            var timeStr = LogDate;
            var indexOfColon = text.IndexOf(": ");

            rtbLog.AppendText(timeStr);

            if (_recorderStatusTranslationKeys.Contains(resource))
            {
                var type = text.Substring(0, indexOfColon);
                var msg = text.Substring(indexOfColon, text.Length - indexOfColon);

                // set message type
                rtbLog.AppendText(type);
                rtbLog.Select(rtbLog.TextLength - type.Length, type.Length + 1);
                rtbLog.SelectionColor = resource.Equals(I18NKeys.LogRecording)
                    ? rtbLog.SelectionColor.SpotifyPrimaryText()
                    : rtbLog.SelectionColor.SpotifySecondaryText();
                rtbLog.SelectionFont = GetDefaultSelectionFont(FontStyle.Bold);

                // set message msg
                rtbLog.AppendText(msg + Environment.NewLine);
                rtbLog.Select(rtbLog.TextLength - msg.Length, msg.Length);
                rtbLog.SelectionColor = rtbLog.SelectionColor = resource.Equals(I18NKeys.LogDeleting)
                    ? rtbLog.SelectionColor.SpotifySecondaryTextAlternate()
                    : rtbLog.SelectionColor.SpotifySecondaryText();
                rtbLog.SelectionFont = GetDefaultSelectionFont(FontStyle.Regular);
            }
            else
            {
                rtbLog.AppendText(text + Environment.NewLine);
            }

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.ScrollToCaret();

            return $";{timeStr}{text}";
        }

        private void WritePreviousLogsIntoConsole(string[] logs)
        {
            if (logs.Length == 0) return;

            foreach (var log in logs) rtbLog.AppendText(log + Environment.NewLine);

            rtbLog.AppendText(LogDate + Rm.GetString(I18NKeys.LogPreviousLogs) + Environment.NewLine +
                              Environment.NewLine);

            rtbLog.Select(0, rtbLog.TextLength);
            rtbLog.SelectionFont = GetDefaultSelectionFont(FontStyle.Regular);

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.ScrollToCaret();
        }

        private void StartRecording()
        {
            if (_watching) return;
            _watching = true;
            _watcher = new Watcher(this, _audioSession, _userSettings);

            Task.Run(_watcher.Run);

            tip.SetToolTip(lnkSpy, Rm.GetString(I18NKeys.TipStopSying));
            tlSettings.Enabled = false;
            tlAdvanced.Enabled = false;
            timer1.Start();
        }

        private bool IsOutputDirectoryNotFound()
        {
            if (Directory.Exists(_userSettings.OutputPath)) return false;

            MetroMessageBox.Show(this,
                Rm.GetString(I18NKeys.MsgBodyPathNotFound),
                Rm.GetString(I18NKeys.MsgTitlePathNotFound),
                MessageBoxButtons.OK,
                MessageBoxIcon.Question);

            return true;
        }

        private bool IsOutputDirectoryPathTooLong()
        {
            if (!FileManager.IsOutputPathTooLong(_userSettings.OutputPath)) return false;

            MetroMessageBox.Show(this,
                Rm.GetString(I18NKeys.MsgBodyPathTooLong),
                Rm.GetString(I18NKeys.MsgTitlePathTooLong),
                MessageBoxButtons.OK,
                MessageBoxIcon.Question);

            return true;
        }

        private void LnkSpy_Click(object sender, EventArgs e)
        {
            if (!Watcher.Running)
            {
                if (IsOutputDirectoryNotFound()) return;
                if (IsOutputDirectoryPathTooLong()) return;

                tcMenu.SelectedIndex = 0;
                StartRecording();
                UpdateLinkImage(lnkSpy, Resources.off);
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
                UpdateLinkImage(lnkSpy, Resources.on);
                Task.Run(async () => await _analytics.LogAction("recording-session?status=ended"));
            }
        }

        private void UpdateLinkImage(MetroLink icon, Bitmap bmp)
        {
            if (icon.Image == bmp) return;
            icon.Image.Dispose();
            icon.Image = bmp;
            icon.Refresh();
        }

        private Font GetDefaultSelectionFont(FontStyle style)
        {
            return new Font(
                new FontFamily("Courier New"),
                8.25f,
                style
            );
        }

        private bool UpdateAudioVirtualCableDriverImage()
        {
            // requires audioSession and Rm
            lnkAudioVirtualCable.Visible = lnkAudioVirtualCable.Enabled = AudioVirtualCableDriver.IsFound;
            var isDriverInstalled =
                AudioVirtualCableDriver.ExistsInAudioEndPointDevices(_audioSession.AudioMMDevicesManager
                    .AudioEndPointDeviceNames);
            var bmp = isDriverInstalled ? Resources.remove_device : Resources.add_device;
            UpdateLinkImage(lnkAudioVirtualCable, bmp);
            var msgTooltip = isDriverInstalled
                ? I18NKeys.TipUninstallVirtualCableDriver
                : I18NKeys.TipInstallVirtualCableDriver;
            tip.SetToolTip(lnkAudioVirtualCable, Rm.GetString(msgTooltip));
            return isDriverInstalled;
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

            if (!Watcher.Running && !Watcher.Ready) StopRecording();
        }

        private void RbFormat_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            var mediaFormatIndex = (int) (rbMp3.Checked ? MediaFormat.Mp3 : MediaFormat.Wav);
            if (Settings.Default.settings_media_audio_format == mediaFormatIndex || !rb.Checked) return;

            var mediaFormat = rb?.Tag?.ToString().ToMediaFormat() ?? MediaFormat.Mp3;
            _userSettings.MediaFormat = mediaFormat;
            Settings.Default.settings_media_audio_format = mediaFormatIndex;
            Settings.Default.Save();
            ReloadExternalAPI();
            ReloadBitrateOptions();
            
            Task.Run(async () => await _analytics.LogAction($"media-format?type={mediaFormat.ToString()}"));
        }

        private void RbMediaTagsAPI_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            var mediaTagsAPI = rbLastFMAPI.Checked ? ExternalAPIType.LastFM : ExternalAPIType.Spotify;

            if (Settings.Default.app_selected_external_api_id == (int) mediaTagsAPI || !rb.Checked) return;

            var api = rb?.Tag?.ToString().ToMediaTagsAPI() ?? ExternalAPIType.LastFM;
            SetExternalAPI(api, _userSettings.IsSpotifyAPISet);
            Settings.Default.app_selected_external_api_id = (int) mediaTagsAPI;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"media-tags-api?type={api.ToString()}"));
        }

        private void TgRecordEverything_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.advanced_record_everything == tgRecordEverything.Checked) return;

            chkRecordAds.Enabled = tgRecordEverything.Checked;
            chkRecordAds.Visible = tgRecordEverything.Checked;

            _userSettings.RecordEverythingEnabled = tgRecordEverything.Checked;
            Settings.Default.advanced_record_everything = tgRecordEverything.Checked;
            Settings.Default.Save();

            if (tgRecordEverything.Checked) tgMuteAds.Checked = false;

            Task.Run(async () => await _analytics.LogAction(
                $"record-everything?enabled={tgRecordEverything.GetPropertyThreadSafe(c => c.Checked)}&including-ads={_userSettings.RecordAdsEnabled}"));
        }

        private void ChkRecordAds_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.advanced_record_everything_and_ads_enabled == chkRecordAds.Checked) return;

            _userSettings.RecordAdsEnabled = chkRecordAds.Checked;
            Settings.Default.advanced_record_everything_and_ads_enabled = chkRecordAds.Checked;
            Settings.Default.Save();

            Task.Run(async () => await _analytics.LogAction(
                $"record-everything?enabled={_userSettings.RecordEverythingEnabled}&including-ads={chkRecordAds.GetPropertyThreadSafe(c => c.Checked)}"));
        }

        private void TgMuteAds_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.settings_mute_ads_enabled == tgMuteAds.Checked) return;

            _userSettings.MuteAdsEnabled = tgMuteAds.Checked;
            Settings.Default.settings_mute_ads_enabled = tgMuteAds.Checked;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction($"mute-ads?enabled={tgMuteAds.GetPropertyThreadSafe(c => c.Checked)}"));
        }

        private void TgListenToSpotifyPlayback_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.advanced_watcher_listen_to_spotify_playback_enabled ==
                tgListenToSpotifyPlayback.Checked) return;

            _userSettings.ListenToSpotifyPlaybackEnabled = tgListenToSpotifyPlayback.Checked;
            Settings.Default.advanced_watcher_listen_to_spotify_playback_enabled = tgListenToSpotifyPlayback.Checked;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"listen-to-spotify-playback?enabled={tgListenToSpotifyPlayback.GetPropertyThreadSafe(c => c.Checked)}"));
        }

        private void TgAddFolders_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.advanced_file_group_media_in_folders_enabled == tgAddFolders.Checked) return;

            _userSettings.GroupByFoldersEnabled = tgAddFolders.Checked;
            Settings.Default.advanced_file_group_media_in_folders_enabled = tgAddFolders.Checked;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"group-by-folders?enabled={tgAddFolders.GetPropertyThreadSafe(c => c.Checked)}"));
        }

        private void TgAddSeparators_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.advanced_file_replace_space_by_underscore_enabled == tgAddSeparators.Checked) return;

            _userSettings.TrackTitleSeparator = tgAddSeparators.Checked ? "_" : " ";
            Settings.Default.advanced_file_replace_space_by_underscore_enabled = tgAddSeparators.Checked;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"track-title-separator?enabled={tgAddSeparators.GetPropertyThreadSafe(c => c.Checked)}"));
        }
        
        private void TgAlbumTrackNumberToFilePrefix_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.advanced_file_album_track_number_prefix_enabled == tgAlbumTrackNumberToFilePrefix.Checked) return;

            _userSettings.AlbumTrackNumberInfrontOfFileEnabled = tgAlbumTrackNumberToFilePrefix.Checked;
            Settings.Default.advanced_file_album_track_number_prefix_enabled = tgAlbumTrackNumberToFilePrefix.Checked;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"album-track-number-in-front-of-files?enabled={tgAlbumTrackNumberToFilePrefix.GetPropertyThreadSafe(c => c.Checked)}"));
        }

        private void TgCounterToFilePrefix_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.advanced_file_counter_number_prefix_enabled == tgCounterToFilePrefix.Checked) return;

            _userSettings.OrderNumberInfrontOfFileEnabled = tgCounterToFilePrefix.Checked;
            Settings.Default.advanced_file_counter_number_prefix_enabled = tgCounterToFilePrefix.Checked;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"order-number-in-front-of-files?enabled={tgCounterToFilePrefix.GetPropertyThreadSafe(c => c.Checked)}"));
        }

        private void TgCounterToMediaTag_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.advanced_id3_counter_number_as_track_number_enabled == tgCounterToMediaTag.Checked) return;

            _userSettings.OrderNumberInMediaTagEnabled = tgCounterToMediaTag.Checked;
            Settings.Default.advanced_id3_counter_number_as_track_number_enabled = tgCounterToMediaTag.Checked;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"order-number-in-media-tags?enabled={tgCounterToMediaTag.GetPropertyThreadSafe(c => c.Checked)}"));
        }

        private void TgRecordOverRecordings_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.advanced_record_over_recordings_enabled == tgRecordOverRecordings.Checked) return;

            chkRecordDuplicateRecordings.Enabled = tgRecordOverRecordings.Checked;
            chkRecordDuplicateRecordings.Visible = tgRecordOverRecordings.Checked;

            Settings.Default.advanced_record_over_recordings_and_duplicate_enabled =
                chkRecordDuplicateRecordings.Checked;
            Settings.Default.advanced_record_over_recordings_enabled = tgRecordOverRecordings.Checked;
            Settings.Default.Save();

            _userSettings.RecordRecordingsStatus = Settings.Default.GetRecordRecordingsStatus();

            Task.Run(async () =>
                await _analytics.LogAction(
                    $"record-recordings-status?status={_userSettings.RecordRecordingsStatus}&overwrite={tgRecordOverRecordings.GetPropertyThreadSafe(c => c.Checked)}"));
        }

        private void ChkRecordDuplicateRecordings_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.advanced_record_over_recordings_and_duplicate_enabled ==
                chkRecordDuplicateRecordings.Checked) return;

            Settings.Default.advanced_record_over_recordings_and_duplicate_enabled =
                chkRecordDuplicateRecordings.Checked;
            Settings.Default.Save();

            _userSettings.RecordRecordingsStatus = Settings.Default.GetRecordRecordingsStatus();

            Task.Run(async () =>
                await _analytics.LogAction(
                    $"record-recordings-status?status={_userSettings.RecordRecordingsStatus}&duplicate={chkRecordDuplicateRecordings.GetPropertyThreadSafe(c => c.Checked)}"));
        }

        private void FrmEspionSpotify_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Watcher.Ready || !Watcher.Running) return;

            if (MetroMessageBox.Show(this,
                    Rm.GetString(I18NKeys.MsgBodyCantQuit),
                    Rm.GetString(I18NKeys.MsgTitleCantQuit),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            Watcher.Running = false;
            _watcher.Dispose();
            _audioSession.Dispose();

            Task.Run(async () => await _analytics.LogAction("exit"));

#if !DEBUG
            Environment.Exit(0);
#endif
        }

        private void LnkPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = string.IsNullOrEmpty(txtPath.Text)
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
                : Path.GetDirectoryName(txtPath.Text);

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK) txtPath.Text = folderBrowserDialog.SelectedPath;
        }

        private void TxtPath_TextChanged(object sender, EventArgs e)
        {
            var path = FileManager.GetCleanPath(txtPath.Text);
            if (Settings.Default.settings_output_path == path) return;

            _userSettings.OutputPath = path;
            Settings.Default.settings_output_path = path;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"set-output-folder?path={path}"));
        }

        private void CbBitRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            var savedBitrate = Settings.Default.settings_media_bitrate_quality;
            if (savedBitrate == cbBitRate.SelectedIndex || savedBitrate > cbBitRate.DataBindings.Count) return;

            _userSettings.Bitrate = cbBitRate.SelectedItem.ToKeyValuePair<LAMEPreset, string>().Key;
            Settings.Default.settings_media_bitrate_quality = cbBitRate.SelectedIndex;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"bitrate?selected={cbBitRate.GetPropertyThreadSafe(c => c.SelectedValue)}"));
        }

        private void CbAudioDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedDeviceID = cbAudioDevices.SelectedItem.ToKeyValuePair<string, string>().Key;

            if (selectedDeviceID == null || Settings.Default.app_selected_audio_device_id == selectedDeviceID) return;

            _userSettings.AudioEndPointDeviceID = selectedDeviceID;
            _audioSession.AudioMMDevicesManager.RefreshSelectedDevice(selectedDeviceID);
            UpdateAudioEndPointFields(_audioSession.AudioDeviceVolume,
                _audioSession.AudioMMDevicesManager.AudioEndPointDeviceName);
            Settings.Default.app_selected_audio_device_id = selectedDeviceID;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"audioEndPointDevice?selected={cbAudioDevices.GetPropertyThreadSafe(c => c.SelectedValue)}"));
        }

        private void LnkNumMinus_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control && txtRecordingNum.Mask.Length > 1)
            {
                txtRecordingNum.Mask = txtRecordingNum.Mask.Substring(1);
                _userSettings.OrderNumberMask = txtRecordingNum.Mask;
                Settings.Default.app_counter_number_mask = txtRecordingNum.Mask;
                Settings.Default.Save();
            }
            else if (_userSettings.InternalOrderNumber - 1 >= 0)
            {
                _userSettings.InternalOrderNumber--;
            }

            txtRecordingNum.Text = _userSettings.InternalOrderNumber.ToString(txtRecordingNum.Mask);
        }

        private void LnkNumPlus_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control && txtRecordingNum.Mask.Length < 6)
            {
                txtRecordingNum.Mask = $"{txtRecordingNum.Mask ?? "000"}0";
                _userSettings.OrderNumberMask = txtRecordingNum.Mask;
                Settings.Default.app_counter_number_mask = txtRecordingNum.Mask;
                Settings.Default.Save();
            }
            else if (_userSettings.InternalOrderNumber + 1 <= _userSettings.OrderNumberMax)
            {
                _userSettings.InternalOrderNumber++;
            }

            txtRecordingNum.Text = _userSettings.InternalOrderNumber.ToString(txtRecordingNum.Mask);
        }

        private void LnkDirectory_Click(object sender, EventArgs e)
        {
            if (IsOutputDirectoryNotFound()) return;

            Process.Start("explorer.exe", txtPath.Text);
            Task.Run(async () => await _analytics.LogAction("open-output-folder"));
        }

        private void TbMinTime_ValueChanged(object sender, EventArgs e)
        {
            var value = tbMinTime.Value * 5;
            _userSettings.MinimumRecordedLengthSeconds = value;

            var min = _userSettings.MinimumRecordedLengthSeconds / 60;
            var sec = _userSettings.MinimumRecordedLengthSeconds % 60;
            lblMinTime.Text = min + @":" + sec.ToString("00");

            if (Settings.Default.settings_media_minimum_recorded_length_in_seconds == value) return;

            Settings.Default.settings_media_minimum_recorded_length_in_seconds = value;
            Settings.Default.Save();
            Task.Run(async () => await _analytics.LogAction($"minimum-media-time?value={value}"));
        }

        private void TbVolumeWin_ValueChanged(object sender, EventArgs e)
        {
            if (_audioSession.AudioMMDevicesManager.AudioEndPointDeviceMute ?? false)
                _audioSession.AudioMMDevicesManager.AudioEndPointDevice.AudioEndpointVolume.Mute = false;

            var volume = tbVolumeWin.Value + tbVolumeWin.Value % 2;
            _audioSession.SetAudioDeviceVolume(volume);
            lblVolume.Text = volume + @"%";

            if (volume == 0)
            {
                if (iconVolume.BackgroundImage != Resources.volmute) iconVolume.BackgroundImage = Resources.volmute;
            }
            else if (volume > 0 && volume < 30)
            {
                if (iconVolume.BackgroundImage != Resources.voldown) iconVolume.BackgroundImage = Resources.voldown;
            }
            else
            {
                if (iconVolume.BackgroundImage != Resources.volup) iconVolume.BackgroundImage = Resources.volup;
            }
        }

        private void Focus_Hover(object sender, EventArgs e)
        {
            var ctrl = (Control) sender;
            ctrl.Focus();
        }

        private void cbLanguage_SelectedValueChanged(object sender, EventArgs e)
        {
            var language = cbLanguage.SelectedItem.ToKeyValuePair<LanguageType, string>().Key.ToString();
            if (language == Settings.Default.settings_language) return;

            Settings.Default.settings_language = language;
            Settings.Default.Save();
            SetLanguage();
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"language?selected={cbLanguage.GetPropertyThreadSafe(c => c.SelectedValue)}"));
        }

        private void TcMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Settings.Default.app_tab_number_selected == tcMenu.SelectedIndex) return;

            Settings.Default.app_tab_number_selected = tcMenu.SelectedIndex;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction($"tab?selected={tcMenu.GetPropertyThreadSafe(c => c.SelectedTab.Text)}"));
        }

        private void LnkRelease_Click(object sender, EventArgs e)
        {
            GitHub.Update();
        }

        private void TxtRecordingTimer_Leave(object sender, EventArgs e)
        {
            _userSettings.RecordingTimer = txtRecordingTimer.Text;
        }

        private void TxtRecordingNum_Leave(object sender, EventArgs e)
        {
            _userSettings.InternalOrderNumber = int.Parse(txtRecordingNum.Text);
        }

        private void LnkVAD_Click(object sender, EventArgs e)
        {
            lnkAudioVirtualCable.Enabled = false;
            if (!(lnkAudioVirtualCable.Visible = AudioVirtualCableDriver.IsFound)) return;
            if (!AudioVirtualCableDriver.SetupDriver())
                MetroMessageBox.Show(this,
                    Rm.GetString(I18NKeys.MsgBodyDriverInstallationFailed),
                    "Audio Virtual Driver",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Question);
            lnkAudioVirtualCable.Enabled = true;
        }

        public void UpdateAudioDevicesDataSource()
        {
            cbAudioDevices.SetPropertyThreadSafe(() => SetAudioEndPointDevicesDropDown());
        }

        private void CbAudioDevices_DataSourceChanged(object sender, EventArgs e)
        {
            var isDriverInstalled = UpdateAudioVirtualCableDriverImage();
            UpdateAudioEndPointFields(_audioSession.AudioDeviceVolume,
                _audioSession.AudioMMDevicesManager.AudioEndPointDeviceName);
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"audio-virtual-cable-driver?status={(isDriverInstalled ? "installed" : "uninstalled")}"));
        }

        private void LnkFAQ_Click(object sender, EventArgs e)
        {
            Process.Start(GitHub.WEBSITE_FAQ_URL);
            Task.Run(async () => await _analytics.LogAction("faq"));
        }

        private void LnkDonate_Click(object sender, EventArgs e)
        {
            Process.Start(GitHub.WEBSITE_DONATE_URL);
            Task.Run(async () => await _analytics.LogAction("donate"));
        }

        private void LnkSpotifyCredentials_Click(object sender, EventArgs e)
        {
            if (_frmSpotifyApiCredentials == null || _frmSpotifyApiCredentials.IsDisposed)
                _frmSpotifyApiCredentials = new FrmSpotifyAPICredentials(_analytics);

            var result = _frmSpotifyApiCredentials.ShowDialog(this);

            SetSpotifyAPIOption();

            if (result == DialogResult.No)
            {
                rbLastFMAPI.Checked = true;
            }

            if (result != DialogResult.Cancel)
            {
                var api = result == DialogResult.Yes ? ExternalAPIType.Spotify : ExternalAPIType.LastFM;
                SetExternalAPI(api, _userSettings.IsSpotifyAPISet);
            }

            _frmSpotifyApiCredentials.Dispose();
        }

        private void TgExtraTitleToSubtitle_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.advanced_id3_extra_title_as_subtitle_enabled == tgExtraTitleToSubtitle.Checked) return;

            _userSettings.ExtraTitleToSubtitleEnabled = tgExtraTitleToSubtitle.Checked;
            Settings.Default.advanced_id3_extra_title_as_subtitle_enabled = tgExtraTitleToSubtitle.Checked;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"move-extra-title-to-subtitle?enabled={tgExtraTitleToSubtitle.GetPropertyThreadSafe(c => c.Checked)}"));
        }

        private void TgUpdateRecordingsID3Tags_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            notifyIcon.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void TgMinimizeToSystemTray_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.settings_minimize_to_system_tray_enabled == tgMinimizeToSystemTray.Checked) return;

            _userSettings.MinimizeToSystemTrayEnabled = tgMinimizeToSystemTray.Checked;
            Settings.Default.settings_minimize_to_system_tray_enabled = tgMinimizeToSystemTray.Checked;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"mimimize-to-system-tray?enabled={tgMinimizeToSystemTray.GetPropertyThreadSafe(c => c.Checked)}"));
        }

        private void FrmEspionSpotify_Resize(object sender, EventArgs e)
        {
            if (!tgMinimizeToSystemTray.Checked) return;
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
            else if (WindowState == FormWindowState.Normal)
            {
                notifyIcon.Visible = false;
            }
        }

        private void tgForceSpotifyToSkip_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings.Default.advanced_watcher_force_spotify_to_skip == tgForceSpotifyToSkip.Checked) return;

            _userSettings.ForceSpotifyToSkipEnabled = tgForceSpotifyToSkip.Checked;
            Settings.Default.advanced_watcher_force_spotify_to_skip = tgForceSpotifyToSkip.Checked;
            Settings.Default.Save();
            Task.Run(async () =>
                await _analytics.LogAction(
                    $"force-spotify-to-skip?enabled={tgForceSpotifyToSkip.GetPropertyThreadSafe(c => c.Checked)}"));
        }
    }
}