using MetroFramework.Controls;
using MetroFramework.Forms;

namespace EspionSpotify
{
    sealed partial class FrmEspionSpotify
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmEspionSpotify));
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tcMenu = new MetroFramework.Controls.MetroTabControl();
            this.tabRecord = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.lnkDirectory = new MetroFramework.Controls.MetroLink();
            this.lnkClear = new MetroFramework.Controls.MetroLink();
            this.lblSoundCard = new MetroFramework.Controls.MetroLabel();
            this.lblVolume = new MetroFramework.Controls.MetroLabel();
            this.tbVolumeWin = new EspionSpotify.Controls.MetroTrackBar();
            this.iconVolume = new MetroFramework.Controls.MetroPanel();
            this.tlpConsoleFrame = new System.Windows.Forms.TableLayoutPanel();
            this.tlpConsole = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblPlayingTitle = new MetroFramework.Controls.MetroLabel();
            this.iconSpotify = new System.Windows.Forms.Panel();
            this.lblRecordedTime = new MetroFramework.Controls.MetroLabel();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.tlSettings = new System.Windows.Forms.TableLayoutPanel();
            this.lblLanguage = new MetroFramework.Controls.MetroLabel();
            this.lblBitRate = new MetroFramework.Controls.MetroLabel();
            this.lblPath = new MetroFramework.Controls.MetroLabel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.lnkPath = new MetroFramework.Controls.MetroLink();
            this.txtPath = new MetroFramework.Controls.MetroTextBox();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.cbBitRate = new EspionSpotify.Controls.MetroComboBox();
            this.cbLanguage = new EspionSpotify.Controls.MetroComboBox();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.tbMinTime = new EspionSpotify.Controls.MetroTrackBar();
            this.lblMinTime = new MetroFramework.Controls.MetroLabel();
            this.lblFormat = new MetroFramework.Controls.MetroLabel();
            this.lblMinLength = new MetroFramework.Controls.MetroLabel();
            this.tableLayoutPanel13 = new System.Windows.Forms.TableLayoutPanel();
            this.rbWav = new MetroFramework.Controls.MetroRadioButton();
            this.rbMp3 = new MetroFramework.Controls.MetroRadioButton();
            this.tlpAPI = new System.Windows.Forms.TableLayoutPanel();
            this.rbLastFMAPI = new MetroFramework.Controls.MetroRadioButton();
            this.rbSpotifyAPI = new MetroFramework.Controls.MetroRadioButton();
            this.lnkSpotifyCredentials = new MetroFramework.Controls.MetroLink();
            this.lblAds = new MetroFramework.Controls.MetroLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblMuteAds = new MetroFramework.Controls.MetroLabel();
            this.tgMuteAds = new MetroFramework.Controls.MetroToggle();
            this.lblAudioDevice = new MetroFramework.Controls.MetroLabel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.lnkAudioVirtualCable = new MetroFramework.Controls.MetroLink();
            this.cbAudioDevices = new EspionSpotify.Controls.MetroComboBox();
            this.tabAdvanced = new System.Windows.Forms.TabPage();
            this.tlAdvanced = new System.Windows.Forms.TableLayoutPanel();
            this.lblSpy = new MetroFramework.Controls.MetroLabel();
            this.lblRecorder = new MetroFramework.Controls.MetroLabel();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.tgRecordUnkownTrackType = new MetroFramework.Controls.MetroToggle();
            this.lblEndingSongDelay = new MetroFramework.Controls.MetroLabel();
            this.lblRecordUnknownTrackType = new MetroFramework.Controls.MetroLabel();
            this.tgEndingSongDelay = new MetroFramework.Controls.MetroToggle();
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.lblRecordingNum = new MetroFramework.Controls.MetroLabel();
            this.tgAddSeparators = new MetroFramework.Controls.MetroToggle();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.lnkNumPlus = new MetroFramework.Controls.MetroLink();
            this.lnkNumMinus = new MetroFramework.Controls.MetroLink();
            this.txtRecordingNum = new System.Windows.Forms.MaskedTextBox();
            this.tgNumFiles = new MetroFramework.Controls.MetroToggle();
            this.lblAddSeparators = new MetroFramework.Controls.MetroLabel();
            this.tgAddFolders = new MetroFramework.Controls.MetroToggle();
            this.lblNumFiles = new MetroFramework.Controls.MetroLabel();
            this.lblAddFolders = new MetroFramework.Controls.MetroLabel();
            this.tgRecordOverRecordings = new MetroFramework.Controls.MetroToggle();
            this.lblRecordingTimer = new MetroFramework.Controls.MetroLabel();
            this.txtRecordingTimer = new System.Windows.Forms.MaskedTextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblRecordOverRecordings = new MetroFramework.Controls.MetroLabel();
            this.chkRecordDuplicateRecordings = new MetroFramework.Controls.MetroCheckBox();
            this.lblID3 = new MetroFramework.Controls.MetroLabel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tgExtraTitleToSubtitle = new MetroFramework.Controls.MetroToggle();
            this.lblExtraTitleToSubtitle = new MetroFramework.Controls.MetroLabel();
            this.lblNumTracks = new MetroFramework.Controls.MetroLabel();
            this.tgNumTracks = new MetroFramework.Controls.MetroToggle();
            this.lnkSpy = new MetroFramework.Controls.MetroLink();
            this.tip = new MetroFramework.Components.MetroToolTip();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lnkRelease = new MetroFramework.Controls.MetroLink();
            this.lnkDonate = new MetroFramework.Controls.MetroLink();
            this.lnkFAQ = new MetroFramework.Controls.MetroLink();
            this.tcMenu.SuspendLayout();
            this.tabRecord.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.tlpConsoleFrame.SuspendLayout();
            this.tlpConsole.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tlSettings.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.tableLayoutPanel13.SuspendLayout();
            this.tlpAPI.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tabAdvanced.SuspendLayout();
            this.tlAdvanced.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.tableLayoutPanel14.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "MSG_FOLDER_DIALOG";
            // 
            // tcMenu
            // 
            this.tcMenu.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tcMenu.Controls.Add(this.tabRecord);
            this.tcMenu.Controls.Add(this.tabSettings);
            this.tcMenu.Controls.Add(this.tabAdvanced);
            this.tcMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMenu.FontWeight = MetroFramework.MetroTabControlWeight.Regular;
            this.tcMenu.HotTrack = true;
            this.tcMenu.ItemSize = new System.Drawing.Size(100, 34);
            this.tcMenu.Location = new System.Drawing.Point(20, 70);
            this.tcMenu.Name = "tcMenu";
            this.tcMenu.SelectedIndex = 0;
            this.tcMenu.Size = new System.Drawing.Size(740, 330);
            this.tcMenu.Style = MetroFramework.MetroColorStyle.Green;
            this.tcMenu.TabIndex = 30;
            this.tcMenu.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tcMenu.UseSelectable = true;
            this.tcMenu.SelectedIndexChanged += new System.EventHandler(this.TcMenu_SelectedIndexChanged);
            this.tcMenu.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // tabRecord
            // 
            this.tabRecord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.tabRecord.Controls.Add(this.tableLayoutPanel4);
            this.tabRecord.ForeColor = System.Drawing.Color.White;
            this.tabRecord.Location = new System.Drawing.Point(4, 38);
            this.tabRecord.Margin = new System.Windows.Forms.Padding(0);
            this.tabRecord.Name = "tabRecord";
            this.tabRecord.Padding = new System.Windows.Forms.Padding(1);
            this.tabRecord.Size = new System.Drawing.Size(732, 288);
            this.tabRecord.TabIndex = 0;
            this.tabRecord.Text = "TAB_SPY";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel10, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.tlpConsoleFrame, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.ForeColor = System.Drawing.Color.Red;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(730, 286);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 6;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutPanel10.Controls.Add(this.lnkDirectory, 5, 0);
            this.tableLayoutPanel10.Controls.Add(this.lnkClear, 4, 0);
            this.tableLayoutPanel10.Controls.Add(this.lblSoundCard, 3, 0);
            this.tableLayoutPanel10.Controls.Add(this.lblVolume, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.tbVolumeWin, 2, 0);
            this.tableLayoutPanel10.Controls.Add(this.iconVolume, 0, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 254);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(724, 29);
            this.tableLayoutPanel10.TabIndex = 35;
            // 
            // lnkDirectory
            // 
            this.lnkDirectory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.lnkDirectory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkDirectory.Image = global::EspionSpotify.Properties.Resources.folder;
            this.lnkDirectory.ImageSize = 32;
            this.lnkDirectory.Location = new System.Drawing.Point(672, 0);
            this.lnkDirectory.Margin = new System.Windows.Forms.Padding(0);
            this.lnkDirectory.Name = "lnkDirectory";
            this.lnkDirectory.Size = new System.Drawing.Size(52, 29);
            this.lnkDirectory.TabIndex = 38;
            this.lnkDirectory.UseCustomBackColor = true;
            this.lnkDirectory.UseSelectable = true;
            this.lnkDirectory.Click += new System.EventHandler(this.LnkDirectory_Click);
            this.lnkDirectory.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // lnkClear
            // 
            this.lnkClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkClear.Image = global::EspionSpotify.Properties.Resources.clear;
            this.lnkClear.ImageSize = 32;
            this.lnkClear.Location = new System.Drawing.Point(632, 0);
            this.lnkClear.Margin = new System.Windows.Forms.Padding(0);
            this.lnkClear.Name = "lnkClear";
            this.lnkClear.Size = new System.Drawing.Size(40, 29);
            this.lnkClear.TabIndex = 36;
            this.lnkClear.UseCustomBackColor = true;
            this.lnkClear.UseSelectable = true;
            this.lnkClear.Click += new System.EventHandler(this.LnkClear_Click);
            this.lnkClear.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // lblSoundCard
            // 
            this.lblSoundCard.AutoSize = true;
            this.lblSoundCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSoundCard.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblSoundCard.Location = new System.Drawing.Point(221, 3);
            this.lblSoundCard.Margin = new System.Windows.Forms.Padding(3);
            this.lblSoundCard.Name = "lblSoundCard";
            this.lblSoundCard.Size = new System.Drawing.Size(408, 23);
            this.lblSoundCard.TabIndex = 28;
            this.lblSoundCard.Text = "LBL_SOUND_COMPONENT";
            this.lblSoundCard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSoundCard.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVolume.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblVolume.Location = new System.Drawing.Point(43, 0);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(34, 29);
            this.lblVolume.TabIndex = 9;
            this.lblVolume.Text = "000%";
            this.lblVolume.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVolume.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tbVolumeWin
            // 
            this.tbVolumeWin.BackColor = System.Drawing.Color.Transparent;
            this.tbVolumeWin.Cursor = System.Windows.Forms.Cursors.Default;
            this.tbVolumeWin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbVolumeWin.Location = new System.Drawing.Point(83, 3);
            this.tbVolumeWin.Name = "tbVolumeWin";
            this.tbVolumeWin.Size = new System.Drawing.Size(132, 23);
            this.tbVolumeWin.TabIndex = 11;
            this.tbVolumeWin.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tbVolumeWin.Value = 0;
            this.tbVolumeWin.ValueChanged += new System.EventHandler(this.TbVolumeWin_ValueChanged);
            this.tbVolumeWin.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // iconVolume
            // 
            this.iconVolume.BackgroundImage = global::EspionSpotify.Properties.Resources.volmute;
            this.iconVolume.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.iconVolume.HorizontalScrollbarBarColor = true;
            this.iconVolume.HorizontalScrollbarHighlightOnWheel = false;
            this.iconVolume.HorizontalScrollbarSize = 10;
            this.iconVolume.Location = new System.Drawing.Point(3, 3);
            this.iconVolume.Name = "iconVolume";
            this.iconVolume.Size = new System.Drawing.Size(34, 23);
            this.iconVolume.TabIndex = 33;
            this.iconVolume.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.iconVolume.VerticalScrollbarBarColor = true;
            this.iconVolume.VerticalScrollbarHighlightOnWheel = false;
            this.iconVolume.VerticalScrollbarSize = 10;
            // 
            // tlpConsoleFrame
            // 
            this.tlpConsoleFrame.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlpConsoleFrame.ColumnCount = 1;
            this.tlpConsoleFrame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpConsoleFrame.Controls.Add(this.tlpConsole, 0, 0);
            this.tlpConsoleFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpConsoleFrame.Location = new System.Drawing.Point(0, 0);
            this.tlpConsoleFrame.Margin = new System.Windows.Forms.Padding(0);
            this.tlpConsoleFrame.Name = "tlpConsoleFrame";
            this.tlpConsoleFrame.RowCount = 1;
            this.tlpConsoleFrame.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpConsoleFrame.Size = new System.Drawing.Size(730, 251);
            this.tlpConsoleFrame.TabIndex = 36;
            // 
            // tlpConsole
            // 
            this.tlpConsole.ColumnCount = 1;
            this.tlpConsole.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpConsole.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tlpConsole.Controls.Add(this.rtbLog, 0, 1);
            this.tlpConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpConsole.Location = new System.Drawing.Point(1, 1);
            this.tlpConsole.Margin = new System.Windows.Forms.Padding(0);
            this.tlpConsole.Name = "tlpConsole";
            this.tlpConsole.RowCount = 2;
            this.tlpConsole.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpConsole.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpConsole.Size = new System.Drawing.Size(728, 249);
            this.tlpConsole.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52F));
            this.tableLayoutPanel3.Controls.Add(this.lblPlayingTitle, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.iconSpotify, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblRecordedTime, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(728, 35);
            this.tableLayoutPanel3.TabIndex = 34;
            // 
            // lblPlayingTitle
            // 
            this.lblPlayingTitle.AutoSize = true;
            this.lblPlayingTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.lblPlayingTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPlayingTitle.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblPlayingTitle.ForeColor = System.Drawing.Color.White;
            this.lblPlayingTitle.Location = new System.Drawing.Point(343, 0);
            this.lblPlayingTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblPlayingTitle.Name = "lblPlayingTitle";
            this.lblPlayingTitle.Size = new System.Drawing.Size(51, 35);
            this.lblPlayingTitle.TabIndex = 34;
            this.lblPlayingTitle.Text = "Spotify";
            this.lblPlayingTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPlayingTitle.UseCustomBackColor = true;
            this.lblPlayingTitle.UseCustomForeColor = true;
            // 
            // iconSpotify
            // 
            this.iconSpotify.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.iconSpotify.BackgroundImage = global::EspionSpotify.Properties.Resources.pause;
            this.iconSpotify.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.iconSpotify.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iconSpotify.Location = new System.Drawing.Point(308, 6);
            this.iconSpotify.Margin = new System.Windows.Forms.Padding(0, 6, 6, 6);
            this.iconSpotify.Name = "iconSpotify";
            this.iconSpotify.Size = new System.Drawing.Size(29, 23);
            this.iconSpotify.TabIndex = 35;
            // 
            // lblRecordedTime
            // 
            this.lblRecordedTime.AutoSize = true;
            this.lblRecordedTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.lblRecordedTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRecordedTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(187)))), ((int)(((byte)(88)))));
            this.lblRecordedTime.Location = new System.Drawing.Point(308, 0);
            this.lblRecordedTime.Margin = new System.Windows.Forms.Padding(0);
            this.lblRecordedTime.Name = "lblRecordedTime";
            this.lblRecordedTime.Size = new System.Drawing.Size(1, 35);
            this.lblRecordedTime.TabIndex = 36;
            this.lblRecordedTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblRecordedTime.UseCustomBackColor = true;
            this.lblRecordedTime.UseCustomForeColor = true;
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(179)))), ((int)(((byte)(179)))));
            this.rtbLog.Location = new System.Drawing.Point(0, 35);
            this.rtbLog.Margin = new System.Windows.Forms.Padding(0);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(728, 214);
            this.rtbLog.TabIndex = 32;
            this.rtbLog.Text = "";
            // 
            // tabSettings
            // 
            this.tabSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.tabSettings.Controls.Add(this.tlSettings);
            this.tabSettings.ForeColor = System.Drawing.Color.White;
            this.tabSettings.Location = new System.Drawing.Point(4, 38);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(1);
            this.tabSettings.Size = new System.Drawing.Size(732, 288);
            this.tabSettings.TabIndex = 1;
            this.tabSettings.Text = "TAB_SETTINGS";
            // 
            // tlSettings
            // 
            this.tlSettings.AutoScroll = true;
            this.tlSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.tlSettings.ColumnCount = 3;
            this.tlSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlSettings.Controls.Add(this.lblLanguage, 0, 5);
            this.tlSettings.Controls.Add(this.lblBitRate, 0, 2);
            this.tlSettings.Controls.Add(this.lblPath, 0, 0);
            this.tlSettings.Controls.Add(this.tableLayoutPanel8, 2, 0);
            this.tlSettings.Controls.Add(this.tableLayoutPanel12, 2, 2);
            this.tlSettings.Controls.Add(this.cbLanguage, 2, 5);
            this.tlSettings.Controls.Add(this.tableLayoutPanel9, 2, 3);
            this.tlSettings.Controls.Add(this.lblFormat, 0, 4);
            this.tlSettings.Controls.Add(this.lblMinLength, 0, 3);
            this.tlSettings.Controls.Add(this.tableLayoutPanel13, 2, 4);
            this.tlSettings.Controls.Add(this.lblAds, 0, 6);
            this.tlSettings.Controls.Add(this.tableLayoutPanel1, 2, 6);
            this.tlSettings.Controls.Add(this.lblAudioDevice, 0, 1);
            this.tlSettings.Controls.Add(this.tableLayoutPanel5, 2, 1);
            this.tlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tlSettings.Location = new System.Drawing.Point(1, 1);
            this.tlSettings.Name = "tlSettings";
            this.tlSettings.Padding = new System.Windows.Forms.Padding(0, 10, 10, 15);
            this.tlSettings.RowCount = 8;
            this.tlSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlSettings.Size = new System.Drawing.Size(730, 286);
            this.tlSettings.TabIndex = 2;
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLanguage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblLanguage.Location = new System.Drawing.Point(3, 191);
            this.lblLanguage.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(125, 29);
            this.lblLanguage.TabIndex = 43;
            this.lblLanguage.Text = "LBL_LANGUAGE";
            this.lblLanguage.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblLanguage.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lblBitRate
            // 
            this.lblBitRate.AutoSize = true;
            this.lblBitRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBitRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblBitRate.Location = new System.Drawing.Point(3, 80);
            this.lblBitRate.Name = "lblBitRate";
            this.lblBitRate.Size = new System.Drawing.Size(125, 35);
            this.lblBitRate.TabIndex = 6;
            this.lblBitRate.Text = "LBL_BITRATE";
            this.lblBitRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblBitRate.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblPath.Location = new System.Drawing.Point(3, 10);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(125, 35);
            this.lblPath.TabIndex = 3;
            this.lblPath.Text = "LBL_PATH";
            this.lblPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblPath.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.Controls.Add(this.lnkPath, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.txtPath, 0, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(144, 13);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(573, 29);
            this.tableLayoutPanel8.TabIndex = 35;
            // 
            // lnkPath
            // 
            this.lnkPath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkPath.Image = global::EspionSpotify.Properties.Resources.folder;
            this.lnkPath.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lnkPath.ImageSize = 28;
            this.lnkPath.Location = new System.Drawing.Point(533, 0);
            this.lnkPath.Margin = new System.Windows.Forms.Padding(0);
            this.lnkPath.Name = "lnkPath";
            this.lnkPath.Size = new System.Drawing.Size(40, 29);
            this.lnkPath.TabIndex = 46;
            this.lnkPath.UseCustomBackColor = true;
            this.lnkPath.UseSelectable = true;
            this.lnkPath.Click += new System.EventHandler(this.LnkPath_Click);
            this.lnkPath.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // txtPath
            // 
            this.txtPath.BackColor = System.Drawing.Color.Black;
            // 
            // 
            // 
            this.txtPath.CustomButton.Image = null;
            this.txtPath.CustomButton.Location = new System.Drawing.Point(505, 1);
            this.txtPath.CustomButton.Name = "";
            this.txtPath.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.txtPath.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtPath.CustomButton.TabIndex = 1;
            this.txtPath.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtPath.CustomButton.UseSelectable = true;
            this.txtPath.CustomButton.Visible = false;
            this.txtPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPath.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.txtPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.txtPath.Lines = new string[0];
            this.txtPath.Location = new System.Drawing.Point(0, 0);
            this.txtPath.Margin = new System.Windows.Forms.Padding(0);
            this.txtPath.MaxLength = 32767;
            this.txtPath.Name = "txtPath";
            this.txtPath.PasswordChar = '\0';
            this.txtPath.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtPath.SelectedText = "";
            this.txtPath.SelectionLength = 0;
            this.txtPath.SelectionStart = 0;
            this.txtPath.ShortcutsEnabled = true;
            this.txtPath.Size = new System.Drawing.Size(533, 29);
            this.txtPath.Style = MetroFramework.MetroColorStyle.Green;
            this.txtPath.TabIndex = 34;
            this.txtPath.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.txtPath.UseCustomBackColor = true;
            this.txtPath.UseSelectable = true;
            this.txtPath.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtPath.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.txtPath.TextChanged += new System.EventHandler(this.TxtPath_TextChanged);
            this.txtPath.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 1;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.Controls.Add(this.cbBitRate, 0, 0);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(144, 83);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 1;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(573, 29);
            this.tableLayoutPanel12.TabIndex = 37;
            // 
            // cbBitRate
            // 
            this.cbBitRate.BackColor = System.Drawing.Color.Black;
            this.cbBitRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbBitRate.ForeColor = System.Drawing.Color.White;
            this.cbBitRate.FormattingEnabled = true;
            this.cbBitRate.ItemHeight = 23;
            this.cbBitRate.Location = new System.Drawing.Point(0, 0);
            this.cbBitRate.Margin = new System.Windows.Forms.Padding(0);
            this.cbBitRate.Name = "cbBitRate";
            this.cbBitRate.Size = new System.Drawing.Size(573, 29);
            this.cbBitRate.Style = MetroFramework.MetroColorStyle.Green;
            this.cbBitRate.TabIndex = 37;
            this.cbBitRate.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.cbBitRate.UseCustomBackColor = true;
            this.cbBitRate.UseSelectable = true;
            this.cbBitRate.SelectedIndexChanged += new System.EventHandler(this.CbBitRate_SelectedIndexChanged);
            this.cbBitRate.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // cbLanguage
            // 
            this.cbLanguage.BackColor = System.Drawing.Color.Black;
            this.cbLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbLanguage.ForeColor = System.Drawing.Color.White;
            this.cbLanguage.FormattingEnabled = true;
            this.cbLanguage.ItemHeight = 23;
            this.cbLanguage.Location = new System.Drawing.Point(144, 188);
            this.cbLanguage.Name = "cbLanguage";
            this.cbLanguage.Size = new System.Drawing.Size(573, 29);
            this.cbLanguage.Style = MetroFramework.MetroColorStyle.Green;
            this.cbLanguage.TabIndex = 44;
            this.cbLanguage.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.cbLanguage.UseCustomBackColor = true;
            this.cbLanguage.UseSelectable = true;
            this.cbLanguage.SelectedIndexChanged += new System.EventHandler(this.CbLanguage_SelectedIndexChanged);
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel9.Controls.Add(this.tbMinTime, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.lblMinTime, 0, 0);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(144, 115);
            this.tableLayoutPanel9.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 1;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(573, 35);
            this.tableLayoutPanel9.TabIndex = 11;
            // 
            // tbMinTime
            // 
            this.tbMinTime.BackColor = System.Drawing.Color.Transparent;
            this.tbMinTime.Cursor = System.Windows.Forms.Cursors.Default;
            this.tbMinTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbMinTime.Location = new System.Drawing.Point(39, 0);
            this.tbMinTime.Margin = new System.Windows.Forms.Padding(0);
            this.tbMinTime.Maximum = 24;
            this.tbMinTime.Name = "tbMinTime";
            this.tbMinTime.Size = new System.Drawing.Size(534, 35);
            this.tbMinTime.TabIndex = 17;
            this.tbMinTime.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tbMinTime.Value = 6;
            this.tbMinTime.ValueChanged += new System.EventHandler(this.TbMinTime_ValueChanged);
            this.tbMinTime.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // lblMinTime
            // 
            this.lblMinTime.AutoSize = true;
            this.lblMinTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMinTime.Location = new System.Drawing.Point(3, 0);
            this.lblMinTime.Name = "lblMinTime";
            this.lblMinTime.Size = new System.Drawing.Size(33, 35);
            this.lblMinTime.TabIndex = 16;
            this.lblMinTime.Text = "0:30";
            this.lblMinTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMinTime.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFormat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblFormat.Location = new System.Drawing.Point(3, 150);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(125, 35);
            this.lblFormat.TabIndex = 9;
            this.lblFormat.Text = "LBL_FORMAT";
            this.lblFormat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblFormat.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lblMinLength
            // 
            this.lblMinLength.AutoSize = true;
            this.lblMinLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMinLength.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblMinLength.Location = new System.Drawing.Point(3, 115);
            this.lblMinLength.Name = "lblMinLength";
            this.lblMinLength.Size = new System.Drawing.Size(125, 35);
            this.lblMinLength.TabIndex = 10;
            this.lblMinLength.Text = "LBL_MIN_LENGTH";
            this.lblMinLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblMinLength.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tableLayoutPanel13
            // 
            this.tableLayoutPanel13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.tableLayoutPanel13.ColumnCount = 4;
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel13.Controls.Add(this.rbWav, 1, 0);
            this.tableLayoutPanel13.Controls.Add(this.rbMp3, 1, 0);
            this.tableLayoutPanel13.Controls.Add(this.tlpAPI, 3, 0);
            this.tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel13.ForeColor = System.Drawing.Color.White;
            this.tableLayoutPanel13.Location = new System.Drawing.Point(144, 150);
            this.tableLayoutPanel13.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            this.tableLayoutPanel13.RowCount = 1;
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel13.Size = new System.Drawing.Size(573, 35);
            this.tableLayoutPanel13.TabIndex = 40;
            // 
            // rbWav
            // 
            this.rbWav.AutoSize = true;
            this.rbWav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbWav.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.rbWav.Location = new System.Drawing.Point(63, 3);
            this.rbWav.Name = "rbWav";
            this.rbWav.Size = new System.Drawing.Size(54, 29);
            this.rbWav.Style = MetroFramework.MetroColorStyle.Green;
            this.rbWav.TabIndex = 40;
            this.rbWav.Tag = "wav";
            this.rbWav.Text = "WAV";
            this.rbWav.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.rbWav.UseSelectable = true;
            this.rbWav.CheckedChanged += new System.EventHandler(this.RbFormat_CheckedChanged);
            // 
            // rbMp3
            // 
            this.rbMp3.AutoSize = true;
            this.rbMp3.Checked = true;
            this.rbMp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbMp3.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.rbMp3.Location = new System.Drawing.Point(3, 3);
            this.rbMp3.Name = "rbMp3";
            this.rbMp3.Size = new System.Drawing.Size(54, 29);
            this.rbMp3.Style = MetroFramework.MetroColorStyle.Green;
            this.rbMp3.TabIndex = 39;
            this.rbMp3.TabStop = true;
            this.rbMp3.Tag = "mp3";
            this.rbMp3.Text = "MP3";
            this.rbMp3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMp3.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.rbMp3.UseSelectable = true;
            this.rbMp3.CheckedChanged += new System.EventHandler(this.RbFormat_CheckedChanged);
            // 
            // tlpAPI
            // 
            this.tlpAPI.ColumnCount = 4;
            this.tlpAPI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAPI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAPI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAPI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAPI.Controls.Add(this.rbLastFMAPI, 1, 0);
            this.tlpAPI.Controls.Add(this.rbSpotifyAPI, 2, 0);
            this.tlpAPI.Controls.Add(this.lnkSpotifyCredentials, 3, 0);
            this.tlpAPI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAPI.Location = new System.Drawing.Point(120, 0);
            this.tlpAPI.Margin = new System.Windows.Forms.Padding(0);
            this.tlpAPI.Name = "tlpAPI";
            this.tlpAPI.RowCount = 1;
            this.tlpAPI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAPI.Size = new System.Drawing.Size(453, 35);
            this.tlpAPI.TabIndex = 41;
            // 
            // rbLastFMAPI
            // 
            this.rbLastFMAPI.AutoSize = true;
            this.rbLastFMAPI.Checked = true;
            this.rbLastFMAPI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbLastFMAPI.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.rbLastFMAPI.Location = new System.Drawing.Point(217, 3);
            this.rbLastFMAPI.Name = "rbLastFMAPI";
            this.rbLastFMAPI.Size = new System.Drawing.Size(95, 29);
            this.rbLastFMAPI.Style = MetroFramework.MetroColorStyle.Green;
            this.rbLastFMAPI.TabIndex = 41;
            this.rbLastFMAPI.TabStop = true;
            this.rbLastFMAPI.Tag = "LastFM";
            this.rbLastFMAPI.Text = "LastFM API";
            this.rbLastFMAPI.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.rbLastFMAPI.UseSelectable = true;
            this.rbLastFMAPI.CheckedChanged += new System.EventHandler(this.RbMediaTagsAPI_CheckedChanged);
            // 
            // rbSpotifyAPI
            // 
            this.rbSpotifyAPI.AutoSize = true;
            this.rbSpotifyAPI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbSpotifyAPI.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.rbSpotifyAPI.ForeColor = System.Drawing.Color.DarkGray;
            this.rbSpotifyAPI.Location = new System.Drawing.Point(318, 3);
            this.rbSpotifyAPI.Name = "rbSpotifyAPI";
            this.rbSpotifyAPI.Size = new System.Drawing.Size(92, 29);
            this.rbSpotifyAPI.Style = MetroFramework.MetroColorStyle.Green;
            this.rbSpotifyAPI.TabIndex = 42;
            this.rbSpotifyAPI.Tag = "Spotify";
            this.rbSpotifyAPI.Text = "Spotify API";
            this.rbSpotifyAPI.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.rbSpotifyAPI.UseSelectable = true;
            this.rbSpotifyAPI.CheckedChanged += new System.EventHandler(this.RbMediaTagsAPI_CheckedChanged);
            // 
            // lnkSpotifyCredentials
            // 
            this.lnkSpotifyCredentials.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkSpotifyCredentials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkSpotifyCredentials.Image = global::EspionSpotify.Properties.Resources.key;
            this.lnkSpotifyCredentials.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lnkSpotifyCredentials.ImageSize = 28;
            this.lnkSpotifyCredentials.Location = new System.Drawing.Point(413, 0);
            this.lnkSpotifyCredentials.Margin = new System.Windows.Forms.Padding(0);
            this.lnkSpotifyCredentials.Name = "lnkSpotifyCredentials";
            this.lnkSpotifyCredentials.Size = new System.Drawing.Size(40, 35);
            this.lnkSpotifyCredentials.TabIndex = 47;
            this.lnkSpotifyCredentials.UseCustomBackColor = true;
            this.lnkSpotifyCredentials.UseSelectable = true;
            this.lnkSpotifyCredentials.Click += new System.EventHandler(this.LnkSpotifyCredentials_Click);
            // 
            // lblAds
            // 
            this.lblAds.AutoSize = true;
            this.lblAds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAds.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblAds.Location = new System.Drawing.Point(3, 226);
            this.lblAds.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblAds.Name = "lblAds";
            this.lblAds.Size = new System.Drawing.Size(125, 29);
            this.lblAds.TabIndex = 46;
            this.lblAds.Text = "LBL_ADS";
            this.lblAds.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblAds.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblMuteAds, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tgMuteAds, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(144, 223);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(573, 29);
            this.tableLayoutPanel1.TabIndex = 47;
            // 
            // lblMuteAds
            // 
            this.lblMuteAds.AutoSize = true;
            this.lblMuteAds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMuteAds.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblMuteAds.Location = new System.Drawing.Point(3, 0);
            this.lblMuteAds.Name = "lblMuteAds";
            this.lblMuteAds.Size = new System.Drawing.Size(467, 29);
            this.lblMuteAds.TabIndex = 43;
            this.lblMuteAds.Text = "LBL_MUTE_ADS";
            this.lblMuteAds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMuteAds.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tgMuteAds
            // 
            this.tgMuteAds.AutoSize = true;
            this.tgMuteAds.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tgMuteAds.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgMuteAds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgMuteAds.Location = new System.Drawing.Point(476, 3);
            this.tgMuteAds.Name = "tgMuteAds";
            this.tgMuteAds.Size = new System.Drawing.Size(94, 23);
            this.tgMuteAds.Style = MetroFramework.MetroColorStyle.Green;
            this.tgMuteAds.TabIndex = 44;
            this.tgMuteAds.Text = "Off";
            this.tgMuteAds.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tgMuteAds.UseSelectable = true;
            this.tgMuteAds.CheckedChanged += new System.EventHandler(this.TgMuteAds_CheckedChanged);
            // 
            // lblAudioDevice
            // 
            this.lblAudioDevice.AutoSize = true;
            this.lblAudioDevice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblAudioDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAudioDevice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblAudioDevice.Location = new System.Drawing.Point(3, 45);
            this.lblAudioDevice.Name = "lblAudioDevice";
            this.lblAudioDevice.Size = new System.Drawing.Size(125, 35);
            this.lblAudioDevice.TabIndex = 48;
            this.lblAudioDevice.Text = "LBL_AUDIO_DEVICE";
            this.lblAudioDevice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblAudioDevice.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.lnkAudioVirtualCable, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.cbAudioDevices, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(144, 48);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(573, 29);
            this.tableLayoutPanel5.TabIndex = 49;
            // 
            // lnkAudioVirtualCable
            // 
            this.lnkAudioVirtualCable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkAudioVirtualCable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkAudioVirtualCable.Image = global::EspionSpotify.Properties.Resources.add_device;
            this.lnkAudioVirtualCable.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lnkAudioVirtualCable.ImageSize = 28;
            this.lnkAudioVirtualCable.Location = new System.Drawing.Point(533, 0);
            this.lnkAudioVirtualCable.Margin = new System.Windows.Forms.Padding(0);
            this.lnkAudioVirtualCable.Name = "lnkAudioVirtualCable";
            this.lnkAudioVirtualCable.Size = new System.Drawing.Size(40, 29);
            this.lnkAudioVirtualCable.TabIndex = 47;
            this.lnkAudioVirtualCable.UseCustomBackColor = true;
            this.lnkAudioVirtualCable.UseSelectable = true;
            this.lnkAudioVirtualCable.Click += new System.EventHandler(this.LnkVAD_Click);
            // 
            // cbAudioDevices
            // 
            this.cbAudioDevices.BackColor = System.Drawing.Color.Black;
            this.cbAudioDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbAudioDevices.ForeColor = System.Drawing.Color.White;
            this.cbAudioDevices.FormattingEnabled = true;
            this.cbAudioDevices.ItemHeight = 23;
            this.cbAudioDevices.Location = new System.Drawing.Point(0, 0);
            this.cbAudioDevices.Margin = new System.Windows.Forms.Padding(0);
            this.cbAudioDevices.Name = "cbAudioDevices";
            this.cbAudioDevices.Size = new System.Drawing.Size(533, 29);
            this.cbAudioDevices.Style = MetroFramework.MetroColorStyle.Green;
            this.cbAudioDevices.TabIndex = 37;
            this.cbAudioDevices.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.cbAudioDevices.UseCustomBackColor = true;
            this.cbAudioDevices.UseSelectable = true;
            this.cbAudioDevices.SelectedIndexChanged += new System.EventHandler(this.CbAudioDevices_SelectedIndexChanged);
            this.cbAudioDevices.DataSourceChanged += new System.EventHandler(this.CbAudioDevices_DataSourceChanged);
            // 
            // tabAdvanced
            // 
            this.tabAdvanced.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.tabAdvanced.Controls.Add(this.tlAdvanced);
            this.tabAdvanced.ForeColor = System.Drawing.Color.White;
            this.tabAdvanced.Location = new System.Drawing.Point(4, 38);
            this.tabAdvanced.Name = "tabAdvanced";
            this.tabAdvanced.Padding = new System.Windows.Forms.Padding(1);
            this.tabAdvanced.Size = new System.Drawing.Size(732, 288);
            this.tabAdvanced.TabIndex = 1;
            this.tabAdvanced.Text = "TAB_ADVANCED";
            // 
            // tlAdvanced
            // 
            this.tlAdvanced.AutoScroll = true;
            this.tlAdvanced.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.tlAdvanced.ColumnCount = 3;
            this.tlAdvanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlAdvanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlAdvanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlAdvanced.Controls.Add(this.lblSpy, 0, 1);
            this.tlAdvanced.Controls.Add(this.lblRecorder, 0, 0);
            this.tlAdvanced.Controls.Add(this.tableLayoutPanel11, 2, 1);
            this.tlAdvanced.Controls.Add(this.tableLayoutPanel14, 2, 0);
            this.tlAdvanced.Controls.Add(this.lblID3, 0, 2);
            this.tlAdvanced.Controls.Add(this.tableLayoutPanel6, 2, 2);
            this.tlAdvanced.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlAdvanced.Location = new System.Drawing.Point(1, 1);
            this.tlAdvanced.Name = "tlAdvanced";
            this.tlAdvanced.Padding = new System.Windows.Forms.Padding(0, 10, 15, 15);
            this.tlAdvanced.RowCount = 4;
            this.tlAdvanced.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlAdvanced.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlAdvanced.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlAdvanced.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlAdvanced.Size = new System.Drawing.Size(730, 286);
            this.tlAdvanced.TabIndex = 2;
            // 
            // lblSpy
            // 
            this.lblSpy.AutoSize = true;
            this.lblSpy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSpy.Location = new System.Drawing.Point(3, 190);
            this.lblSpy.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblSpy.Name = "lblSpy";
            this.lblSpy.Size = new System.Drawing.Size(102, 56);
            this.lblSpy.TabIndex = 0;
            this.lblSpy.Text = "LBL_SPY";
            this.lblSpy.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblSpy.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lblRecorder
            // 
            this.lblRecorder.AutoSize = true;
            this.lblRecorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRecorder.Location = new System.Drawing.Point(3, 16);
            this.lblRecorder.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblRecorder.Name = "lblRecorder";
            this.lblRecorder.Size = new System.Drawing.Size(102, 168);
            this.lblRecorder.TabIndex = 1;
            this.lblRecorder.Text = "LBL_RECORDER";
            this.lblRecorder.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblRecorder.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.AutoSize = true;
            this.tableLayoutPanel11.ColumnCount = 2;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel11.Controls.Add(this.tgRecordUnkownTrackType, 1, 1);
            this.tableLayoutPanel11.Controls.Add(this.lblEndingSongDelay, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.lblRecordUnknownTrackType, 0, 1);
            this.tableLayoutPanel11.Controls.Add(this.tgEndingSongDelay, 1, 0);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(121, 187);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 2;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(574, 56);
            this.tableLayoutPanel11.TabIndex = 6;
            // 
            // tgRecordUnkownTrackType
            // 
            this.tgRecordUnkownTrackType.AutoSize = true;
            this.tgRecordUnkownTrackType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgRecordUnkownTrackType.Location = new System.Drawing.Point(477, 31);
            this.tgRecordUnkownTrackType.Name = "tgRecordUnkownTrackType";
            this.tgRecordUnkownTrackType.Size = new System.Drawing.Size(94, 22);
            this.tgRecordUnkownTrackType.Style = MetroFramework.MetroColorStyle.Green;
            this.tgRecordUnkownTrackType.TabIndex = 44;
            this.tgRecordUnkownTrackType.Text = "Off";
            this.tgRecordUnkownTrackType.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tgRecordUnkownTrackType.UseSelectable = true;
            this.tgRecordUnkownTrackType.CheckedChanged += new System.EventHandler(this.TgRecordUnkownTrackType_CheckedChanged);
            // 
            // lblEndingSongDelay
            // 
            this.lblEndingSongDelay.AutoSize = true;
            this.lblEndingSongDelay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEndingSongDelay.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblEndingSongDelay.Location = new System.Drawing.Point(3, 0);
            this.lblEndingSongDelay.Name = "lblEndingSongDelay";
            this.lblEndingSongDelay.Size = new System.Drawing.Size(468, 28);
            this.lblEndingSongDelay.TabIndex = 48;
            this.lblEndingSongDelay.Text = "LBL_ENDING_SONG_DELAY";
            this.lblEndingSongDelay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblEndingSongDelay.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lblRecordUnknownTrackType
            // 
            this.lblRecordUnknownTrackType.AutoSize = true;
            this.lblRecordUnknownTrackType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRecordUnknownTrackType.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblRecordUnknownTrackType.Location = new System.Drawing.Point(3, 28);
            this.lblRecordUnknownTrackType.Name = "lblRecordUnknownTrackType";
            this.lblRecordUnknownTrackType.Size = new System.Drawing.Size(468, 28);
            this.lblRecordUnknownTrackType.TabIndex = 43;
            this.lblRecordUnknownTrackType.Text = "LBL_RECORD_UNKOWN_TRACK_TYPE";
            this.lblRecordUnknownTrackType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRecordUnknownTrackType.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tgEndingSongDelay
            // 
            this.tgEndingSongDelay.AutoSize = true;
            this.tgEndingSongDelay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgEndingSongDelay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgEndingSongDelay.Location = new System.Drawing.Point(477, 3);
            this.tgEndingSongDelay.Name = "tgEndingSongDelay";
            this.tgEndingSongDelay.Size = new System.Drawing.Size(94, 22);
            this.tgEndingSongDelay.Style = MetroFramework.MetroColorStyle.Green;
            this.tgEndingSongDelay.TabIndex = 0;
            this.tgEndingSongDelay.Text = "Off";
            this.tgEndingSongDelay.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tgEndingSongDelay.UseSelectable = true;
            this.tgEndingSongDelay.CheckedChanged += new System.EventHandler(this.TgEndingSongDelay_CheckedChanged);
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.AutoSize = true;
            this.tableLayoutPanel14.ColumnCount = 2;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel14.Controls.Add(this.lblRecordingNum, 0, 1);
            this.tableLayoutPanel14.Controls.Add(this.tgAddSeparators, 1, 4);
            this.tableLayoutPanel14.Controls.Add(this.tableLayoutPanel7, 1, 1);
            this.tableLayoutPanel14.Controls.Add(this.tgNumFiles, 1, 2);
            this.tableLayoutPanel14.Controls.Add(this.lblAddSeparators, 0, 4);
            this.tableLayoutPanel14.Controls.Add(this.tgAddFolders, 1, 3);
            this.tableLayoutPanel14.Controls.Add(this.lblNumFiles, 0, 2);
            this.tableLayoutPanel14.Controls.Add(this.lblAddFolders, 0, 3);
            this.tableLayoutPanel14.Controls.Add(this.tgRecordOverRecordings, 1, 5);
            this.tableLayoutPanel14.Controls.Add(this.lblRecordingTimer, 0, 0);
            this.tableLayoutPanel14.Controls.Add(this.txtRecordingTimer, 1, 0);
            this.tableLayoutPanel14.Controls.Add(this.tableLayoutPanel2, 0, 5);
            this.tableLayoutPanel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel14.Location = new System.Drawing.Point(121, 13);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 6;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(574, 168);
            this.tableLayoutPanel14.TabIndex = 7;
            // 
            // lblRecordingNum
            // 
            this.lblRecordingNum.AutoSize = true;
            this.lblRecordingNum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRecordingNum.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblRecordingNum.Location = new System.Drawing.Point(3, 28);
            this.lblRecordingNum.Name = "lblRecordingNum";
            this.lblRecordingNum.Size = new System.Drawing.Size(468, 28);
            this.lblRecordingNum.TabIndex = 50;
            this.lblRecordingNum.Text = "LBL_RECORDING_NUMBER";
            this.lblRecordingNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRecordingNum.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tgAddSeparators
            // 
            this.tgAddSeparators.AutoSize = true;
            this.tgAddSeparators.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgAddSeparators.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgAddSeparators.Location = new System.Drawing.Point(477, 115);
            this.tgAddSeparators.Name = "tgAddSeparators";
            this.tgAddSeparators.Size = new System.Drawing.Size(94, 22);
            this.tgAddSeparators.Style = MetroFramework.MetroColorStyle.Green;
            this.tgAddSeparators.TabIndex = 45;
            this.tgAddSeparators.Text = "Off";
            this.tgAddSeparators.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tgAddSeparators.UseSelectable = true;
            this.tgAddSeparators.CheckedChanged += new System.EventHandler(this.TgAddSeparators_CheckedChanged);
            this.tgAddSeparators.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.AutoSize = true;
            this.tableLayoutPanel7.ColumnCount = 3;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel7.Controls.Add(this.lnkNumPlus, 2, 0);
            this.tableLayoutPanel7.Controls.Add(this.lnkNumMinus, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.txtRecordingNum, 1, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(474, 28);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(100, 28);
            this.tableLayoutPanel7.TabIndex = 51;
            // 
            // lnkNumPlus
            // 
            this.lnkNumPlus.AutoSize = true;
            this.lnkNumPlus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkNumPlus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkNumPlus.Image = global::EspionSpotify.Properties.Resources.plus;
            this.lnkNumPlus.ImageSize = 18;
            this.lnkNumPlus.Location = new System.Drawing.Point(80, 0);
            this.lnkNumPlus.Margin = new System.Windows.Forms.Padding(0);
            this.lnkNumPlus.Name = "lnkNumPlus";
            this.lnkNumPlus.Size = new System.Drawing.Size(20, 28);
            this.lnkNumPlus.TabIndex = 45;
            this.lnkNumPlus.UseCustomBackColor = true;
            this.lnkNumPlus.UseSelectable = true;
            this.lnkNumPlus.Click += new System.EventHandler(this.LnkNumPlus_Click);
            this.lnkNumPlus.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // lnkNumMinus
            // 
            this.lnkNumMinus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkNumMinus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkNumMinus.Image = global::EspionSpotify.Properties.Resources.minus;
            this.lnkNumMinus.ImageSize = 18;
            this.lnkNumMinus.Location = new System.Drawing.Point(0, 0);
            this.lnkNumMinus.Margin = new System.Windows.Forms.Padding(0);
            this.lnkNumMinus.Name = "lnkNumMinus";
            this.lnkNumMinus.Size = new System.Drawing.Size(20, 28);
            this.lnkNumMinus.TabIndex = 44;
            this.lnkNumMinus.UseCustomBackColor = true;
            this.lnkNumMinus.UseSelectable = true;
            this.lnkNumMinus.Click += new System.EventHandler(this.LnkNumMinus_Click);
            this.lnkNumMinus.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // txtRecordingNum
            // 
            this.txtRecordingNum.BackColor = System.Drawing.Color.Black;
            this.txtRecordingNum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRecordingNum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRecordingNum.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtRecordingNum.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(179)))), ((int)(((byte)(179)))));
            this.txtRecordingNum.Location = new System.Drawing.Point(23, 3);
            this.txtRecordingNum.Mask = "000";
            this.txtRecordingNum.Name = "txtRecordingNum";
            this.txtRecordingNum.PromptChar = '0';
            this.txtRecordingNum.Size = new System.Drawing.Size(54, 25);
            this.txtRecordingNum.SkipLiterals = false;
            this.txtRecordingNum.TabIndex = 46;
            this.txtRecordingNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtRecordingNum.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludePrompt;
            this.txtRecordingNum.Leave += new System.EventHandler(this.TxtRecordingNum_Leave);
            // 
            // tgNumFiles
            // 
            this.tgNumFiles.AutoSize = true;
            this.tgNumFiles.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgNumFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgNumFiles.Location = new System.Drawing.Point(477, 59);
            this.tgNumFiles.Name = "tgNumFiles";
            this.tgNumFiles.Size = new System.Drawing.Size(94, 22);
            this.tgNumFiles.Style = MetroFramework.MetroColorStyle.Green;
            this.tgNumFiles.TabIndex = 47;
            this.tgNumFiles.Text = "Off";
            this.tgNumFiles.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tgNumFiles.UseSelectable = true;
            this.tgNumFiles.CheckedChanged += new System.EventHandler(this.TgNumFiles_CheckedChanged);
            this.tgNumFiles.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // lblAddSeparators
            // 
            this.lblAddSeparators.AutoSize = true;
            this.lblAddSeparators.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAddSeparators.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblAddSeparators.Location = new System.Drawing.Point(3, 112);
            this.lblAddSeparators.Name = "lblAddSeparators";
            this.lblAddSeparators.Size = new System.Drawing.Size(468, 28);
            this.lblAddSeparators.TabIndex = 44;
            this.lblAddSeparators.Text = "LBL_ADD_SEPARATORS";
            this.lblAddSeparators.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAddSeparators.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tgAddFolders
            // 
            this.tgAddFolders.AutoSize = true;
            this.tgAddFolders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgAddFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgAddFolders.Location = new System.Drawing.Point(477, 87);
            this.tgAddFolders.Name = "tgAddFolders";
            this.tgAddFolders.Size = new System.Drawing.Size(94, 22);
            this.tgAddFolders.Style = MetroFramework.MetroColorStyle.Green;
            this.tgAddFolders.TabIndex = 43;
            this.tgAddFolders.Text = "Off";
            this.tgAddFolders.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tgAddFolders.UseSelectable = true;
            this.tgAddFolders.CheckedChanged += new System.EventHandler(this.TgAddFolders_CheckedChanged);
            this.tgAddFolders.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // lblNumFiles
            // 
            this.lblNumFiles.AutoSize = true;
            this.lblNumFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNumFiles.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblNumFiles.Location = new System.Drawing.Point(3, 56);
            this.lblNumFiles.Name = "lblNumFiles";
            this.lblNumFiles.Size = new System.Drawing.Size(468, 28);
            this.lblNumFiles.TabIndex = 46;
            this.lblNumFiles.Text = "LBL_ADD_NUMBERS_TO_FILES";
            this.lblNumFiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNumFiles.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lblAddFolders
            // 
            this.lblAddFolders.AutoSize = true;
            this.lblAddFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAddFolders.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblAddFolders.Location = new System.Drawing.Point(3, 84);
            this.lblAddFolders.Name = "lblAddFolders";
            this.lblAddFolders.Size = new System.Drawing.Size(468, 28);
            this.lblAddFolders.TabIndex = 42;
            this.lblAddFolders.Text = "LBL_ADD_FOLDERS";
            this.lblAddFolders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAddFolders.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tgRecordOverRecordings
            // 
            this.tgRecordOverRecordings.AutoSize = true;
            this.tgRecordOverRecordings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgRecordOverRecordings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgRecordOverRecordings.Location = new System.Drawing.Point(477, 143);
            this.tgRecordOverRecordings.Name = "tgRecordOverRecordings";
            this.tgRecordOverRecordings.Size = new System.Drawing.Size(94, 22);
            this.tgRecordOverRecordings.Style = MetroFramework.MetroColorStyle.Green;
            this.tgRecordOverRecordings.TabIndex = 45;
            this.tgRecordOverRecordings.Text = "Off";
            this.tgRecordOverRecordings.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tgRecordOverRecordings.UseSelectable = true;
            this.tgRecordOverRecordings.CheckedChanged += new System.EventHandler(this.TgRecordOverRecordings_CheckedChanged);
            this.tgRecordOverRecordings.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // lblRecordingTimer
            // 
            this.lblRecordingTimer.AutoSize = true;
            this.lblRecordingTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRecordingTimer.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblRecordingTimer.Location = new System.Drawing.Point(3, 0);
            this.lblRecordingTimer.Name = "lblRecordingTimer";
            this.lblRecordingTimer.Size = new System.Drawing.Size(468, 28);
            this.lblRecordingTimer.TabIndex = 50;
            this.lblRecordingTimer.Text = "LBL_RECORDING_TIMER";
            this.lblRecordingTimer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRecordingTimer.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // txtRecordingTimer
            // 
            this.txtRecordingTimer.BackColor = System.Drawing.Color.Black;
            this.txtRecordingTimer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRecordingTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRecordingTimer.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRecordingTimer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(179)))), ((int)(((byte)(179)))));
            this.txtRecordingTimer.Location = new System.Drawing.Point(477, 3);
            this.txtRecordingTimer.Mask = "00h00m00s";
            this.txtRecordingTimer.Name = "txtRecordingTimer";
            this.txtRecordingTimer.PromptChar = '0';
            this.txtRecordingTimer.Size = new System.Drawing.Size(94, 25);
            this.txtRecordingTimer.TabIndex = 52;
            this.txtRecordingTimer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtRecordingTimer.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludePrompt;
            this.txtRecordingTimer.Leave += new System.EventHandler(this.TxtRecordingTimer_Leave);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.lblRecordOverRecordings, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkRecordDuplicateRecordings, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 140);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(474, 28);
            this.tableLayoutPanel2.TabIndex = 53;
            // 
            // lblRecordOverRecordings
            // 
            this.lblRecordOverRecordings.AutoSize = true;
            this.lblRecordOverRecordings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRecordOverRecordings.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblRecordOverRecordings.Location = new System.Drawing.Point(3, 0);
            this.lblRecordOverRecordings.Name = "lblRecordOverRecordings";
            this.lblRecordOverRecordings.Size = new System.Drawing.Size(182, 28);
            this.lblRecordOverRecordings.TabIndex = 44;
            this.lblRecordOverRecordings.Text = "LBL_RECORD_OVER_RECORDINGS";
            this.lblRecordOverRecordings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRecordOverRecordings.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // chkRecordDuplicateRecordings
            // 
            this.chkRecordDuplicateRecordings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.chkRecordDuplicateRecordings.AutoSize = true;
            this.chkRecordDuplicateRecordings.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRecordDuplicateRecordings.FontWeight = MetroFramework.MetroCheckBoxWeight.Light;
            this.chkRecordDuplicateRecordings.Location = new System.Drawing.Point(191, 3);
            this.chkRecordDuplicateRecordings.Name = "chkRecordDuplicateRecordings";
            this.chkRecordDuplicateRecordings.Size = new System.Drawing.Size(102, 22);
            this.chkRecordDuplicateRecordings.Style = MetroFramework.MetroColorStyle.Green;
            this.chkRecordDuplicateRecordings.TabIndex = 45;
            this.chkRecordDuplicateRecordings.Text = "LBL_DUPLICATE";
            this.chkRecordDuplicateRecordings.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.chkRecordDuplicateRecordings.UseSelectable = true;
            this.chkRecordDuplicateRecordings.CheckedChanged += new System.EventHandler(this.ChkRecordDuplicateRecordings_CheckedChanged);
            // 
            // lblID3
            // 
            this.lblID3.AutoSize = true;
            this.lblID3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblID3.Location = new System.Drawing.Point(3, 252);
            this.lblID3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblID3.Name = "lblID3";
            this.lblID3.Size = new System.Drawing.Size(102, 100);
            this.lblID3.TabIndex = 8;
            this.lblID3.Text = "LBL_ID3";
            this.lblID3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblID3.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel6.Controls.Add(this.tgExtraTitleToSubtitle, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.lblExtraTitleToSubtitle, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.lblNumTracks, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tgNumTracks, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(121, 249);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 3;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(574, 100);
            this.tableLayoutPanel6.TabIndex = 9;
            // 
            // tgExtraTitleToSubtitle
            // 
            this.tgExtraTitleToSubtitle.AutoSize = true;
            this.tgExtraTitleToSubtitle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgExtraTitleToSubtitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgExtraTitleToSubtitle.Location = new System.Drawing.Point(477, 31);
            this.tgExtraTitleToSubtitle.Name = "tgExtraTitleToSubtitle";
            this.tgExtraTitleToSubtitle.Size = new System.Drawing.Size(94, 22);
            this.tgExtraTitleToSubtitle.Style = MetroFramework.MetroColorStyle.Green;
            this.tgExtraTitleToSubtitle.TabIndex = 51;
            this.tgExtraTitleToSubtitle.Text = "Off";
            this.tgExtraTitleToSubtitle.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tgExtraTitleToSubtitle.UseSelectable = true;
            this.tgExtraTitleToSubtitle.CheckedChanged += new System.EventHandler(this.TgExtraTitleToSubtitle_CheckedChanged);
            // 
            // lblExtraTitleToSubtitle
            // 
            this.lblExtraTitleToSubtitle.AutoSize = true;
            this.lblExtraTitleToSubtitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblExtraTitleToSubtitle.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblExtraTitleToSubtitle.Location = new System.Drawing.Point(3, 28);
            this.lblExtraTitleToSubtitle.Name = "lblExtraTitleToSubtitle";
            this.lblExtraTitleToSubtitle.Size = new System.Drawing.Size(468, 28);
            this.lblExtraTitleToSubtitle.TabIndex = 50;
            this.lblExtraTitleToSubtitle.Text = "LBL_MOVE_EXTRA_TITLE_TO_SUBTITLE";
            this.lblExtraTitleToSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblExtraTitleToSubtitle.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lblNumTracks
            // 
            this.lblNumTracks.AutoSize = true;
            this.lblNumTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNumTracks.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblNumTracks.Location = new System.Drawing.Point(3, 0);
            this.lblNumTracks.Name = "lblNumTracks";
            this.lblNumTracks.Size = new System.Drawing.Size(468, 28);
            this.lblNumTracks.TabIndex = 48;
            this.lblNumTracks.Text = "LBL_ADD_NUMBERS_AS_TRACK";
            this.lblNumTracks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNumTracks.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tgNumTracks
            // 
            this.tgNumTracks.AutoSize = true;
            this.tgNumTracks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgNumTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgNumTracks.Location = new System.Drawing.Point(477, 3);
            this.tgNumTracks.Name = "tgNumTracks";
            this.tgNumTracks.Size = new System.Drawing.Size(94, 22);
            this.tgNumTracks.Style = MetroFramework.MetroColorStyle.Green;
            this.tgNumTracks.TabIndex = 49;
            this.tgNumTracks.Text = "Off";
            this.tgNumTracks.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tgNumTracks.UseSelectable = true;
            this.tgNumTracks.CheckedChanged += new System.EventHandler(this.TgNumTracks_CheckedChanged);
            this.tgNumTracks.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // lnkSpy
            // 
            this.lnkSpy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkSpy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.lnkSpy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkSpy.Image = global::EspionSpotify.Properties.Resources.on;
            this.lnkSpy.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lnkSpy.ImageSize = 0;
            this.lnkSpy.Location = new System.Drawing.Point(623, 50);
            this.lnkSpy.Margin = new System.Windows.Forms.Padding(0);
            this.lnkSpy.Name = "lnkSpy";
            this.lnkSpy.Size = new System.Drawing.Size(133, 43);
            this.lnkSpy.TabIndex = 39;
            this.lnkSpy.UseCustomBackColor = true;
            this.lnkSpy.UseSelectable = true;
            this.lnkSpy.Click += new System.EventHandler(this.LnkSpy_Click);
            this.lnkSpy.MouseHover += new System.EventHandler(this.Focus_Hover);
            // 
            // tip
            // 
            this.tip.Style = MetroFramework.MetroColorStyle.Default;
            this.tip.StyleManager = null;
            this.tip.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // lnkRelease
            // 
            this.lnkRelease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkRelease.Image = global::EspionSpotify.Properties.Resources.release;
            this.lnkRelease.Location = new System.Drawing.Point(626, 5);
            this.lnkRelease.Margin = new System.Windows.Forms.Padding(0);
            this.lnkRelease.Name = "lnkRelease";
            this.lnkRelease.Size = new System.Drawing.Size(18, 18);
            this.lnkRelease.TabIndex = 40;
            this.lnkRelease.UseCustomBackColor = true;
            this.lnkRelease.UseSelectable = true;
            this.lnkRelease.Visible = false;
            this.lnkRelease.Click += new System.EventHandler(this.LnkRelease_Click);
            // 
            // lnkDonate
            // 
            this.lnkDonate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkDonate.Image = global::EspionSpotify.Properties.Resources.heart;
            this.lnkDonate.Location = new System.Drawing.Point(654, 5);
            this.lnkDonate.Margin = new System.Windows.Forms.Padding(0);
            this.lnkDonate.Name = "lnkDonate";
            this.lnkDonate.Size = new System.Drawing.Size(18, 18);
            this.lnkDonate.TabIndex = 42;
            this.lnkDonate.UseCustomBackColor = true;
            this.lnkDonate.UseSelectable = true;
            this.lnkDonate.Click += new System.EventHandler(this.LnkDonate_Click);
            // 
            // lnkFAQ
            // 
            this.lnkFAQ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkFAQ.Image = global::EspionSpotify.Properties.Resources.faq;
            this.lnkFAQ.ImageSize = 12;
            this.lnkFAQ.Location = new System.Drawing.Point(680, 5);
            this.lnkFAQ.Margin = new System.Windows.Forms.Padding(0);
            this.lnkFAQ.Name = "lnkFAQ";
            this.lnkFAQ.Size = new System.Drawing.Size(18, 18);
            this.lnkFAQ.TabIndex = 43;
            this.lnkFAQ.UseCustomBackColor = true;
            this.lnkFAQ.UseSelectable = true;
            this.lnkFAQ.Click += new System.EventHandler(this.LnkFAQ_Click);
            // 
            // FrmEspionSpotify
            // 
            this.AcceptButton = this.lnkSpy;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BackImage = global::EspionSpotify.Properties.Resources.spytify_logo;
            this.BackImagePadding = new System.Windows.Forms.Padding(25, 15, 0, 0);
            this.BackMaxSize = 250;
            this.ClientSize = new System.Drawing.Size(780, 420);
            this.Controls.Add(this.lnkFAQ);
            this.Controls.Add(this.lnkDonate);
            this.Controls.Add(this.lnkRelease);
            this.Controls.Add(this.lnkSpy);
            this.Controls.Add(this.tcMenu);
            this.DisplayHeader = false;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(680, 360);
            this.Name = "FrmEspionSpotify";
            this.Padding = new System.Windows.Forms.Padding(20, 70, 20, 20);
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.None;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Style = MetroFramework.MetroColorStyle.Green;
            this.Text = "Spytify";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmEspionSpotify_FormClosing);
            this.tcMenu.ResumeLayout(false);
            this.tabRecord.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            this.tlpConsoleFrame.ResumeLayout(false);
            this.tlpConsole.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tlSettings.ResumeLayout(false);
            this.tlSettings.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel12.ResumeLayout(false);
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            this.tableLayoutPanel13.ResumeLayout(false);
            this.tableLayoutPanel13.PerformLayout();
            this.tlpAPI.ResumeLayout(false);
            this.tlpAPI.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tabAdvanced.ResumeLayout(false);
            this.tlAdvanced.ResumeLayout(false);
            this.tlAdvanced.PerformLayout();
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel11.PerformLayout();
            this.tableLayoutPanel14.ResumeLayout(false);
            this.tableLayoutPanel14.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private MetroTabControl tcMenu;
        private System.Windows.Forms.TabPage tabRecord;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TableLayoutPanel tlSettings;
        private MetroLabel lblPath;
        private MetroLabel lblMinLength;
        private MetroLabel lblFormat;
        private MetroLabel lblBitRate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private EspionSpotify.Controls.MetroTrackBar tbMinTime;
        private MetroLabel lblMinTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private MetroTextBox txtPath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel12;
        private EspionSpotify.Controls.MetroComboBox cbBitRate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel13;
        private MetroRadioButton rbWav;
        private MetroRadioButton rbMp3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private MetroLabel lblSoundCard;
        private MetroLabel lblVolume;
        private EspionSpotify.Controls.MetroTrackBar tbVolumeWin;
        private MetroPanel iconVolume;
        private MetroLink lnkDirectory;
        private MetroLink lnkClear;
        private MetroLink lnkPath;
        private MetroLink lnkSpy;
        private MetroFramework.Components.MetroToolTip tip;
        private MetroLabel lblLanguage;
        private EspionSpotify.Controls.MetroComboBox cbLanguage;
        private MetroLabel lblAds;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Timer timer1;
        private MetroToggle tgEndingSongDelay;
        private MetroLabel lblEndingSongDelay;
        private MetroLabel lblRecordUnknownTrackType;
        private MetroToggle tgRecordUnkownTrackType;
        private System.Windows.Forms.TabPage tabAdvanced;
        private System.Windows.Forms.TableLayoutPanel tlAdvanced;
        private MetroLabel lblSpy;
        private MetroLabel lblRecorder;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel11;
        private MetroLabel lblMuteAds;
        private MetroToggle tgMuteAds;
        private MetroLink lnkRelease;
        private MetroLabel lblAudioDevice;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private Controls.MetroComboBox cbAudioDevices;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel14;
        private MetroLabel lblRecordingNum;
        private MetroToggle tgAddSeparators;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private MetroLink lnkNumPlus;
        private MetroLink lnkNumMinus;
        private MetroToggle tgNumFiles;
        private MetroLabel lblAddSeparators;
        private MetroToggle tgNumTracks;
        private MetroToggle tgAddFolders;
        private MetroLabel lblNumFiles;
        private MetroLabel lblAddFolders;
        private MetroLabel lblNumTracks;
        private MetroLabel lblRecordOverRecordings;
        private MetroToggle tgRecordOverRecordings;
        private MetroLabel lblRecordingTimer;
        private System.Windows.Forms.MaskedTextBox txtRecordingTimer;
        private System.Windows.Forms.Panel iconSpotify;
        private MetroLabel lblPlayingTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.TableLayoutPanel tlpConsoleFrame;
        private System.Windows.Forms.TableLayoutPanel tlpConsole;
        private System.Windows.Forms.MaskedTextBox txtRecordingNum;
        private MetroLabel lblRecordedTime;
        private System.Windows.Forms.TableLayoutPanel tlpAPI;
        private MetroRadioButton rbLastFMAPI;
        private MetroRadioButton rbSpotifyAPI;
        private MetroLink lnkAudioVirtualCable;
        private MetroLink lnkDonate;
        private MetroLink lnkFAQ;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private MetroCheckBox chkRecordDuplicateRecordings;
        private MetroLink lnkSpotifyCredentials;
        private MetroLabel lblID3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private MetroToggle tgExtraTitleToSubtitle;
        private MetroLabel lblExtraTitleToSubtitle;
    }
}

