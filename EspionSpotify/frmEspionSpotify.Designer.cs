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
            this.tabRecord = new MetroFramework.Controls.MetroTabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.lnkDirectory = new MetroFramework.Controls.MetroLink();
            this.lnkClear = new MetroFramework.Controls.MetroLink();
            this.lblSoundCard = new MetroFramework.Controls.MetroLabel();
            this.lblVolume = new MetroFramework.Controls.MetroLabel();
            this.tbVolumeWin = new MetroFramework.Controls.MetroTrackBar();
            this.iconVolume = new MetroFramework.Controls.MetroPanel();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblPlayingTitle = new MetroFramework.Controls.MetroLabel();
            this.iconSpotify = new System.Windows.Forms.Panel();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLanguage = new MetroFramework.Controls.MetroLabel();
            this.lblCustomize = new MetroFramework.Controls.MetroLabel();
            this.lblBitRate = new MetroFramework.Controls.MetroLabel();
            this.lblPath = new MetroFramework.Controls.MetroLabel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.lnkPath = new MetroFramework.Controls.MetroLink();
            this.txtPath = new MetroFramework.Controls.MetroTextBox();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.cbBitRate = new MetroFramework.Controls.MetroComboBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblRecordingNum = new MetroFramework.Controls.MetroLabel();
            this.lblNumTracks = new MetroFramework.Controls.MetroLabel();
            this.lblNumFiles = new MetroFramework.Controls.MetroLabel();
            this.lblAddSeparators = new MetroFramework.Controls.MetroLabel();
            this.lblAddFolders = new MetroFramework.Controls.MetroLabel();
            this.tgAddFolders = new MetroFramework.Controls.MetroToggle();
            this.tgAddSeparators = new MetroFramework.Controls.MetroToggle();
            this.tgNumFiles = new MetroFramework.Controls.MetroToggle();
            this.tgNumTracks = new MetroFramework.Controls.MetroToggle();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.lnkNumPlus = new MetroFramework.Controls.MetroLink();
            this.lnkNumMinus = new MetroFramework.Controls.MetroLink();
            this.lblNum = new MetroFramework.Controls.MetroLabel();
            this.cbLanguage = new MetroFramework.Controls.MetroComboBox();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.tbMinTime = new MetroFramework.Controls.MetroTrackBar();
            this.lblMinTime = new MetroFramework.Controls.MetroLabel();
            this.lblFormat = new MetroFramework.Controls.MetroLabel();
            this.lblMinLength = new MetroFramework.Controls.MetroLabel();
            this.tableLayoutPanel13 = new System.Windows.Forms.TableLayoutPanel();
            this.rbWav = new MetroFramework.Controls.MetroRadioButton();
            this.rbMp3 = new MetroFramework.Controls.MetroRadioButton();
            this.lblAds = new MetroFramework.Controls.MetroLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tgDisableAds = new MetroFramework.Controls.MetroToggle();
            this.lblDisableAds = new MetroFramework.Controls.MetroLabel();
            this.lnkSpy = new MetroFramework.Controls.MetroLink();
            this.tip = new MetroFramework.Components.MetroToolTip();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tcMenu.SuspendLayout();
            this.tabRecord.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.tableLayoutPanel13.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Veuillez sélectionner l\'emplacement où vous souhaitez sauvegarder les fichiers au" +
    "dio.";
            // 
            // tcMenu
            // 
            this.tcMenu.Controls.Add(this.tabRecord);
            this.tcMenu.Controls.Add(this.tabSettings);
            this.tcMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMenu.Location = new System.Drawing.Point(20, 60);
            this.tcMenu.Name = "tcMenu";
            this.tcMenu.SelectedIndex = 1;
            this.tcMenu.Size = new System.Drawing.Size(760, 420);
            this.tcMenu.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tcMenu.Style = MetroFramework.MetroColorStyle.Green;
            this.tcMenu.TabIndex = 30;
            this.tcMenu.UseSelectable = true;
            this.tcMenu.SelectedIndexChanged += new System.EventHandler(this.tcMenu_SelectedIndexChanged);
            this.tcMenu.MouseHover += new System.EventHandler(this.focus_Hover);
            // 
            // tabRecord
            // 
            this.tabRecord.Controls.Add(this.tableLayoutPanel4);
            this.tabRecord.HorizontalScrollbarBarColor = true;
            this.tabRecord.HorizontalScrollbarHighlightOnWheel = false;
            this.tabRecord.HorizontalScrollbarSize = 10;
            this.tabRecord.Location = new System.Drawing.Point(4, 38);
            this.tabRecord.Name = "tabRecord";
            this.tabRecord.Padding = new System.Windows.Forms.Padding(1);
            this.tabRecord.Size = new System.Drawing.Size(752, 378);
            this.tabRecord.TabIndex = 0;
            this.tabRecord.Text = "TAB_SPY";
            this.tabRecord.VerticalScrollbarBarColor = true;
            this.tabRecord.VerticalScrollbarHighlightOnWheel = false;
            this.tabRecord.VerticalScrollbarSize = 10;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel10, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.rtbLog, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(750, 376);
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
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutPanel10.Controls.Add(this.lnkDirectory, 5, 0);
            this.tableLayoutPanel10.Controls.Add(this.lnkClear, 4, 0);
            this.tableLayoutPanel10.Controls.Add(this.lblSoundCard, 3, 0);
            this.tableLayoutPanel10.Controls.Add(this.lblVolume, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.tbVolumeWin, 2, 0);
            this.tableLayoutPanel10.Controls.Add(this.iconVolume, 0, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 344);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(744, 29);
            this.tableLayoutPanel10.TabIndex = 35;
            // 
            // lnkDirectory
            // 
            this.lnkDirectory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.lnkDirectory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkDirectory.Image = global::EspionSpotify.Properties.Resources.folder;
            this.lnkDirectory.ImageSize = 32;
            this.lnkDirectory.Location = new System.Drawing.Point(700, 0);
            this.lnkDirectory.Margin = new System.Windows.Forms.Padding(0);
            this.lnkDirectory.Name = "lnkDirectory";
            this.lnkDirectory.Size = new System.Drawing.Size(44, 29);
            this.lnkDirectory.TabIndex = 38;
            this.lnkDirectory.UseSelectable = true;
            this.lnkDirectory.Click += new System.EventHandler(this.lnkDirectory_Click);
            this.lnkDirectory.MouseHover += new System.EventHandler(this.focus_Hover);
            // 
            // lnkClear
            // 
            this.lnkClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkClear.Image = global::EspionSpotify.Properties.Resources.clear;
            this.lnkClear.ImageSize = 32;
            this.lnkClear.Location = new System.Drawing.Point(660, 0);
            this.lnkClear.Margin = new System.Windows.Forms.Padding(0);
            this.lnkClear.Name = "lnkClear";
            this.lnkClear.Size = new System.Drawing.Size(40, 29);
            this.lnkClear.TabIndex = 36;
            this.lnkClear.UseSelectable = true;
            this.lnkClear.Click += new System.EventHandler(this.lnkClear_Click);
            this.lnkClear.MouseHover += new System.EventHandler(this.focus_Hover);
            // 
            // lblSoundCard
            // 
            this.lblSoundCard.AutoSize = true;
            this.lblSoundCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSoundCard.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblSoundCard.Location = new System.Drawing.Point(228, 3);
            this.lblSoundCard.Margin = new System.Windows.Forms.Padding(3);
            this.lblSoundCard.Name = "lblSoundCard";
            this.lblSoundCard.Size = new System.Drawing.Size(429, 23);
            this.lblSoundCard.TabIndex = 28;
            this.lblSoundCard.Text = "LBL_SOUND_COMPONENT";
            this.lblSoundCard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // 
            // tbVolumeWin
            // 
            this.tbVolumeWin.BackColor = System.Drawing.Color.Transparent;
            this.tbVolumeWin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbVolumeWin.Location = new System.Drawing.Point(83, 3);
            this.tbVolumeWin.Name = "tbVolumeWin";
            this.tbVolumeWin.Size = new System.Drawing.Size(139, 23);
            this.tbVolumeWin.TabIndex = 11;
            this.tbVolumeWin.Value = 0;
            this.tbVolumeWin.ValueChanged += new System.EventHandler(this.tbVolumeWin_ValueChanged);
            this.tbVolumeWin.MouseHover += new System.EventHandler(this.focus_Hover);
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
            this.iconVolume.VerticalScrollbarBarColor = true;
            this.iconVolume.VerticalScrollbarHighlightOnWheel = false;
            this.iconVolume.VerticalScrollbarSize = 10;
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.Black;
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLog.ForeColor = System.Drawing.Color.Silver;
            this.rtbLog.Location = new System.Drawing.Point(0, 35);
            this.rtbLog.Margin = new System.Windows.Forms.Padding(0);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(750, 306);
            this.rtbLog.TabIndex = 32;
            this.rtbLog.Text = "";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.Black;
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52F));
            this.tableLayoutPanel3.Controls.Add(this.lblPlayingTitle, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.iconSpotify, 2, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(750, 35);
            this.tableLayoutPanel3.TabIndex = 34;
            // 
            // lblPlayingTitle
            // 
            this.lblPlayingTitle.AutoSize = true;
            this.lblPlayingTitle.BackColor = System.Drawing.Color.Black;
            this.lblPlayingTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPlayingTitle.ForeColor = System.Drawing.Color.SpringGreen;
            this.lblPlayingTitle.Location = new System.Drawing.Point(354, 0);
            this.lblPlayingTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblPlayingTitle.Name = "lblPlayingTitle";
            this.lblPlayingTitle.Size = new System.Drawing.Size(49, 35);
            this.lblPlayingTitle.TabIndex = 34;
            this.lblPlayingTitle.Text = "Spotify";
            this.lblPlayingTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPlayingTitle.UseCustomBackColor = true;
            this.lblPlayingTitle.UseCustomForeColor = true;
            // 
            // iconSpotify
            // 
            this.iconSpotify.BackColor = System.Drawing.Color.Black;
            this.iconSpotify.BackgroundImage = global::EspionSpotify.Properties.Resources.pause;
            this.iconSpotify.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.iconSpotify.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iconSpotify.Location = new System.Drawing.Point(319, 6);
            this.iconSpotify.Margin = new System.Windows.Forms.Padding(0, 6, 6, 6);
            this.iconSpotify.Name = "iconSpotify";
            this.iconSpotify.Size = new System.Drawing.Size(29, 23);
            this.iconSpotify.TabIndex = 35;
            // 
            // tabSettings
            // 
            this.tabSettings.BackColor = System.Drawing.Color.White;
            this.tabSettings.Controls.Add(this.tableLayoutPanel5);
            this.tabSettings.Location = new System.Drawing.Point(4, 38);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(1);
            this.tabSettings.Size = new System.Drawing.Size(752, 378);
            this.tabSettings.TabIndex = 1;
            this.tabSettings.Text = "TAB_SETTINGS";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.lblLanguage, 0, 6);
            this.tableLayoutPanel5.Controls.Add(this.lblCustomize, 0, 5);
            this.tableLayoutPanel5.Controls.Add(this.lblBitRate, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.lblPath, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel8, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel12, 2, 1);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel2, 2, 5);
            this.tableLayoutPanel5.Controls.Add(this.cbLanguage, 2, 6);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel9, 2, 2);
            this.tableLayoutPanel5.Controls.Add(this.lblFormat, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.lblMinLength, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel13, 2, 3);
            this.tableLayoutPanel5.Controls.Add(this.lblAds, 0, 4);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel1, 2, 4);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.tableLayoutPanel5.RowCount = 7;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(750, 376);
            this.tableLayoutPanel5.TabIndex = 3;
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLanguage.Location = new System.Drawing.Point(3, 337);
            this.lblLanguage.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(114, 39);
            this.lblLanguage.TabIndex = 43;
            this.lblLanguage.Text = "LBL_LANGUAGE";
            this.lblLanguage.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblCustomize
            // 
            this.lblCustomize.AutoSize = true;
            this.lblCustomize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCustomize.Location = new System.Drawing.Point(3, 191);
            this.lblCustomize.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblCustomize.Name = "lblCustomize";
            this.lblCustomize.Size = new System.Drawing.Size(114, 140);
            this.lblCustomize.TabIndex = 41;
            this.lblCustomize.Text = "LBL_CUSTOMIZE";
            this.lblCustomize.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblBitRate
            // 
            this.lblBitRate.AutoSize = true;
            this.lblBitRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBitRate.Location = new System.Drawing.Point(3, 45);
            this.lblBitRate.Name = "lblBitRate";
            this.lblBitRate.Size = new System.Drawing.Size(114, 35);
            this.lblBitRate.TabIndex = 6;
            this.lblBitRate.Text = "LBL_BITRATE";
            this.lblBitRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPath.Location = new System.Drawing.Point(3, 10);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(114, 35);
            this.lblPath.TabIndex = 3;
            this.lblPath.Text = "LBL_PATH";
            this.lblPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel8.Controls.Add(this.lnkPath, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.txtPath, 0, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(130, 10);
            this.tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(620, 35);
            this.tableLayoutPanel8.TabIndex = 35;
            // 
            // lnkPath
            // 
            this.lnkPath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkPath.Image = global::EspionSpotify.Properties.Resources.folder;
            this.lnkPath.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lnkPath.ImageSize = 28;
            this.lnkPath.Location = new System.Drawing.Point(583, 3);
            this.lnkPath.Name = "lnkPath";
            this.lnkPath.Size = new System.Drawing.Size(34, 29);
            this.lnkPath.TabIndex = 46;
            this.lnkPath.UseSelectable = true;
            this.lnkPath.Click += new System.EventHandler(this.lnkPath_Click);
            this.lnkPath.MouseHover += new System.EventHandler(this.focus_Hover);
            // 
            // txtPath
            // 
            // 
            // 
            // 
            this.txtPath.CustomButton.Image = null;
            this.txtPath.CustomButton.Location = new System.Drawing.Point(546, 1);
            this.txtPath.CustomButton.Name = "";
            this.txtPath.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.txtPath.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtPath.CustomButton.TabIndex = 1;
            this.txtPath.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtPath.CustomButton.UseSelectable = true;
            this.txtPath.CustomButton.Visible = false;
            this.txtPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPath.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.txtPath.Lines = new string[0];
            this.txtPath.Location = new System.Drawing.Point(3, 3);
            this.txtPath.MaxLength = 32767;
            this.txtPath.Name = "txtPath";
            this.txtPath.PasswordChar = '\0';
            this.txtPath.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtPath.SelectedText = "";
            this.txtPath.SelectionLength = 0;
            this.txtPath.SelectionStart = 0;
            this.txtPath.ShortcutsEnabled = true;
            this.txtPath.Size = new System.Drawing.Size(574, 29);
            this.txtPath.Style = MetroFramework.MetroColorStyle.Green;
            this.txtPath.TabIndex = 34;
            this.txtPath.UseSelectable = true;
            this.txtPath.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtPath.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.txtPath.TextChanged += new System.EventHandler(this.txtPath_TextChanged);
            this.txtPath.MouseHover += new System.EventHandler(this.focus_Hover);
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 1;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.Controls.Add(this.cbBitRate, 0, 0);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(133, 48);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 1;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(614, 29);
            this.tableLayoutPanel12.TabIndex = 37;
            // 
            // cbBitRate
            // 
            this.cbBitRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbBitRate.FormattingEnabled = true;
            this.cbBitRate.ItemHeight = 23;
            this.cbBitRate.Location = new System.Drawing.Point(0, 0);
            this.cbBitRate.Margin = new System.Windows.Forms.Padding(0);
            this.cbBitRate.Name = "cbBitRate";
            this.cbBitRate.Size = new System.Drawing.Size(614, 29);
            this.cbBitRate.Style = MetroFramework.MetroColorStyle.Green;
            this.cbBitRate.TabIndex = 37;
            this.cbBitRate.UseSelectable = true;
            this.cbBitRate.SelectedIndexChanged += new System.EventHandler(this.cbBitRate_SelectedIndexChanged);
            this.cbBitRate.MouseHover += new System.EventHandler(this.focus_Hover);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.lblRecordingNum, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.lblNumTracks, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.lblNumFiles, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.lblAddSeparators, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblAddFolders, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tgAddFolders, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tgAddSeparators, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.tgNumFiles, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.tgNumTracks, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel7, 1, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(133, 188);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(614, 140);
            this.tableLayoutPanel2.TabIndex = 42;
            // 
            // lblRecordingNum
            // 
            this.lblRecordingNum.AutoSize = true;
            this.lblRecordingNum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRecordingNum.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblRecordingNum.Location = new System.Drawing.Point(3, 112);
            this.lblRecordingNum.Name = "lblRecordingNum";
            this.lblRecordingNum.Size = new System.Drawing.Size(508, 28);
            this.lblRecordingNum.TabIndex = 50;
            this.lblRecordingNum.Text = "LBL_RECORDING_NUMBER";
            this.lblRecordingNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNumTracks
            // 
            this.lblNumTracks.AutoSize = true;
            this.lblNumTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNumTracks.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblNumTracks.Location = new System.Drawing.Point(3, 84);
            this.lblNumTracks.Name = "lblNumTracks";
            this.lblNumTracks.Size = new System.Drawing.Size(508, 28);
            this.lblNumTracks.TabIndex = 48;
            this.lblNumTracks.Text = "LBL_ADD_NUMBERS_AS_TRACK";
            this.lblNumTracks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNumFiles
            // 
            this.lblNumFiles.AutoSize = true;
            this.lblNumFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNumFiles.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblNumFiles.Location = new System.Drawing.Point(3, 56);
            this.lblNumFiles.Name = "lblNumFiles";
            this.lblNumFiles.Size = new System.Drawing.Size(508, 28);
            this.lblNumFiles.TabIndex = 46;
            this.lblNumFiles.Text = "LBL_ADD_NUMBERS_TO_FILES";
            this.lblNumFiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAddSeparators
            // 
            this.lblAddSeparators.AutoSize = true;
            this.lblAddSeparators.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAddSeparators.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblAddSeparators.Location = new System.Drawing.Point(3, 28);
            this.lblAddSeparators.Name = "lblAddSeparators";
            this.lblAddSeparators.Size = new System.Drawing.Size(508, 28);
            this.lblAddSeparators.TabIndex = 44;
            this.lblAddSeparators.Text = "LBL_ADD_SEPARATORS";
            this.lblAddSeparators.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAddFolders
            // 
            this.lblAddFolders.AutoSize = true;
            this.lblAddFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAddFolders.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblAddFolders.Location = new System.Drawing.Point(3, 0);
            this.lblAddFolders.Name = "lblAddFolders";
            this.lblAddFolders.Size = new System.Drawing.Size(508, 28);
            this.lblAddFolders.TabIndex = 42;
            this.lblAddFolders.Text = "LBL_ADD_FOLDERS";
            this.lblAddFolders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tgAddFolders
            // 
            this.tgAddFolders.AutoSize = true;
            this.tgAddFolders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgAddFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgAddFolders.Location = new System.Drawing.Point(517, 3);
            this.tgAddFolders.Name = "tgAddFolders";
            this.tgAddFolders.Size = new System.Drawing.Size(94, 22);
            this.tgAddFolders.Style = MetroFramework.MetroColorStyle.Green;
            this.tgAddFolders.TabIndex = 43;
            this.tgAddFolders.Text = "Off";
            this.tgAddFolders.UseSelectable = true;
            this.tgAddFolders.CheckedChanged += new System.EventHandler(this.tgAddFolders_CheckedChanged);
            this.tgAddFolders.MouseHover += new System.EventHandler(this.focus_Hover);
            // 
            // tgAddSeparators
            // 
            this.tgAddSeparators.AutoSize = true;
            this.tgAddSeparators.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgAddSeparators.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgAddSeparators.Location = new System.Drawing.Point(517, 31);
            this.tgAddSeparators.Name = "tgAddSeparators";
            this.tgAddSeparators.Size = new System.Drawing.Size(94, 22);
            this.tgAddSeparators.Style = MetroFramework.MetroColorStyle.Green;
            this.tgAddSeparators.TabIndex = 45;
            this.tgAddSeparators.Text = "Off";
            this.tgAddSeparators.UseSelectable = true;
            this.tgAddSeparators.CheckedChanged += new System.EventHandler(this.tgAddSeparators_CheckedChanged);
            this.tgAddSeparators.MouseHover += new System.EventHandler(this.focus_Hover);
            // 
            // tgNumFiles
            // 
            this.tgNumFiles.AutoSize = true;
            this.tgNumFiles.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgNumFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgNumFiles.Location = new System.Drawing.Point(517, 59);
            this.tgNumFiles.Name = "tgNumFiles";
            this.tgNumFiles.Size = new System.Drawing.Size(94, 22);
            this.tgNumFiles.Style = MetroFramework.MetroColorStyle.Green;
            this.tgNumFiles.TabIndex = 47;
            this.tgNumFiles.Text = "Off";
            this.tgNumFiles.UseSelectable = true;
            this.tgNumFiles.CheckedChanged += new System.EventHandler(this.tgNumFiles_CheckedChanged);
            this.tgNumFiles.MouseHover += new System.EventHandler(this.focus_Hover);
            // 
            // tgNumTracks
            // 
            this.tgNumTracks.AutoSize = true;
            this.tgNumTracks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tgNumTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgNumTracks.Location = new System.Drawing.Point(517, 87);
            this.tgNumTracks.Name = "tgNumTracks";
            this.tgNumTracks.Size = new System.Drawing.Size(94, 22);
            this.tgNumTracks.Style = MetroFramework.MetroColorStyle.Green;
            this.tgNumTracks.TabIndex = 49;
            this.tgNumTracks.Text = "Off";
            this.tgNumTracks.UseSelectable = true;
            this.tgNumTracks.CheckedChanged += new System.EventHandler(this.tgNumTracks_CheckedChanged);
            this.tgNumTracks.MouseHover += new System.EventHandler(this.focus_Hover);
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.AutoSize = true;
            this.tableLayoutPanel7.ColumnCount = 3;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel7.Controls.Add(this.lnkNumPlus, 2, 0);
            this.tableLayoutPanel7.Controls.Add(this.lnkNumMinus, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.lblNum, 1, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(517, 115);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(94, 22);
            this.tableLayoutPanel7.TabIndex = 51;
            // 
            // lnkNumPlus
            // 
            this.lnkNumPlus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkNumPlus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lnkNumPlus.Image = global::EspionSpotify.Properties.Resources.plus;
            this.lnkNumPlus.ImageSize = 18;
            this.lnkNumPlus.Location = new System.Drawing.Point(64, 0);
            this.lnkNumPlus.Margin = new System.Windows.Forms.Padding(0);
            this.lnkNumPlus.Name = "lnkNumPlus";
            this.lnkNumPlus.Size = new System.Drawing.Size(30, 22);
            this.lnkNumPlus.TabIndex = 45;
            this.lnkNumPlus.UseSelectable = true;
            this.lnkNumPlus.Click += new System.EventHandler(this.lnkNumPlus_Click);
            this.lnkNumPlus.MouseHover += new System.EventHandler(this.focus_Hover);
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
            this.lnkNumMinus.Size = new System.Drawing.Size(30, 22);
            this.lnkNumMinus.TabIndex = 44;
            this.lnkNumMinus.UseSelectable = true;
            this.lnkNumMinus.Click += new System.EventHandler(this.lnkNumMinus_Click);
            this.lnkNumMinus.MouseHover += new System.EventHandler(this.focus_Hover);
            // 
            // lblNum
            // 
            this.lblNum.AutoSize = true;
            this.lblNum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNum.Location = new System.Drawing.Point(33, 0);
            this.lblNum.Name = "lblNum";
            this.lblNum.Size = new System.Drawing.Size(28, 22);
            this.lblNum.TabIndex = 2;
            this.lblNum.Text = "001";
            this.lblNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNum.TextChanged += new System.EventHandler(this.lblNum_TextChanged);
            // 
            // cbLanguage
            // 
            this.cbLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbLanguage.FormattingEnabled = true;
            this.cbLanguage.ItemHeight = 23;
            this.cbLanguage.Location = new System.Drawing.Point(133, 334);
            this.cbLanguage.Name = "cbLanguage";
            this.cbLanguage.Size = new System.Drawing.Size(614, 29);
            this.cbLanguage.TabIndex = 44;
            this.cbLanguage.UseSelectable = true;
            this.cbLanguage.SelectedIndexChanged += new System.EventHandler(this.cbLanguage_SelectedIndexChanged);
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Controls.Add(this.tbMinTime, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.lblMinTime, 0, 0);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(133, 83);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 1;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(614, 29);
            this.tableLayoutPanel9.TabIndex = 11;
            // 
            // tbMinTime
            // 
            this.tbMinTime.BackColor = System.Drawing.Color.Transparent;
            this.tbMinTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbMinTime.Location = new System.Drawing.Point(42, 3);
            this.tbMinTime.Maximum = 24;
            this.tbMinTime.Name = "tbMinTime";
            this.tbMinTime.Size = new System.Drawing.Size(569, 23);
            this.tbMinTime.TabIndex = 17;
            this.tbMinTime.Text = "metroTrackBar1";
            this.tbMinTime.Value = 6;
            this.tbMinTime.ValueChanged += new System.EventHandler(this.tbMinTime_ValueChanged);
            this.tbMinTime.MouseHover += new System.EventHandler(this.focus_Hover);
            // 
            // lblMinTime
            // 
            this.lblMinTime.AutoSize = true;
            this.lblMinTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMinTime.Location = new System.Drawing.Point(3, 0);
            this.lblMinTime.Name = "lblMinTime";
            this.lblMinTime.Size = new System.Drawing.Size(33, 29);
            this.lblMinTime.TabIndex = 16;
            this.lblMinTime.Text = "0:30";
            this.lblMinTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFormat.Location = new System.Drawing.Point(3, 115);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(114, 35);
            this.lblFormat.TabIndex = 9;
            this.lblFormat.Text = "LBL_FORMAT";
            this.lblFormat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMinLength
            // 
            this.lblMinLength.AutoSize = true;
            this.lblMinLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMinLength.Location = new System.Drawing.Point(3, 80);
            this.lblMinLength.Name = "lblMinLength";
            this.lblMinLength.Size = new System.Drawing.Size(114, 35);
            this.lblMinLength.TabIndex = 10;
            this.lblMinLength.Text = "LBL_MIN_LENGTH";
            this.lblMinLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel13
            // 
            this.tableLayoutPanel13.ColumnCount = 4;
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel13.Controls.Add(this.rbWav, 1, 0);
            this.tableLayoutPanel13.Controls.Add(this.rbMp3, 1, 0);
            this.tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel13.Location = new System.Drawing.Point(133, 118);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            this.tableLayoutPanel13.RowCount = 1;
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel13.Size = new System.Drawing.Size(614, 29);
            this.tableLayoutPanel13.TabIndex = 40;
            // 
            // rbWav
            // 
            this.rbWav.AutoSize = true;
            this.rbWav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbWav.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.rbWav.Location = new System.Drawing.Point(63, 3);
            this.rbWav.Name = "rbWav";
            this.rbWav.Size = new System.Drawing.Size(54, 23);
            this.rbWav.Style = MetroFramework.MetroColorStyle.Green;
            this.rbWav.TabIndex = 40;
            this.rbWav.Tag = "wav";
            this.rbWav.Text = "WAV";
            this.rbWav.UseSelectable = true;
            this.rbWav.CheckedChanged += new System.EventHandler(this.rbFormat_CheckedChanged);
            // 
            // rbMp3
            // 
            this.rbMp3.AutoSize = true;
            this.rbMp3.Checked = true;
            this.rbMp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbMp3.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.rbMp3.Location = new System.Drawing.Point(3, 3);
            this.rbMp3.Name = "rbMp3";
            this.rbMp3.Size = new System.Drawing.Size(54, 23);
            this.rbMp3.Style = MetroFramework.MetroColorStyle.Green;
            this.rbMp3.TabIndex = 39;
            this.rbMp3.TabStop = true;
            this.rbMp3.Tag = "mp3";
            this.rbMp3.Text = "MP3";
            this.rbMp3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMp3.UseSelectable = true;
            this.rbMp3.CheckedChanged += new System.EventHandler(this.rbFormat_CheckedChanged);
            // 
            // lblAds
            // 
            this.lblAds.AutoSize = true;
            this.lblAds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAds.Location = new System.Drawing.Point(3, 150);
            this.lblAds.Name = "lblAds";
            this.lblAds.Size = new System.Drawing.Size(114, 35);
            this.lblAds.TabIndex = 46;
            this.lblAds.Text = "ADS";
            this.lblAds.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tgDisableAds, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblDisableAds, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(133, 153);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(614, 29);
            this.tableLayoutPanel1.TabIndex = 47;
            // 
            // tgDisableAds
            // 
            this.tgDisableAds.Location = new System.Drawing.Point(517, 4);
            this.tgDisableAds.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tgDisableAds.Name = "tgDisableAds";
            this.tgDisableAds.Size = new System.Drawing.Size(94, 21);
            this.tgDisableAds.Style = MetroFramework.MetroColorStyle.Green;
            this.tgDisableAds.TabIndex = 41;
            this.tgDisableAds.Text = "Off";
            this.tgDisableAds.UseSelectable = true;
            this.tgDisableAds.CheckedChanged += new System.EventHandler(this.tgDisableAds_CheckedChanged);
            this.tgDisableAds.Click += new System.EventHandler(this.tgDisableAds_Click);
            // 
            // lblDisableAds
            // 
            this.lblDisableAds.AutoSize = true;
            this.lblDisableAds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDisableAds.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblDisableAds.Location = new System.Drawing.Point(3, 0);
            this.lblDisableAds.Name = "lblDisableAds";
            this.lblDisableAds.Size = new System.Drawing.Size(508, 29);
            this.lblDisableAds.TabIndex = 42;
            this.lblDisableAds.Text = "LBL_DISABLE_ADS";
            this.lblDisableAds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lnkSpy
            // 
            this.lnkSpy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkSpy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.lnkSpy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkSpy.Image = global::EspionSpotify.Properties.Resources.on;
            this.lnkSpy.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lnkSpy.ImageSize = 0;
            this.lnkSpy.Location = new System.Drawing.Point(636, 39);
            this.lnkSpy.Margin = new System.Windows.Forms.Padding(0);
            this.lnkSpy.Name = "lnkSpy";
            this.lnkSpy.Size = new System.Drawing.Size(136, 44);
            this.lnkSpy.TabIndex = 39;
            this.lnkSpy.UseSelectable = true;
            this.lnkSpy.Click += new System.EventHandler(this.lnkSpy_Click);
            this.lnkSpy.MouseHover += new System.EventHandler(this.focus_Hover);
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
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FrmEspionSpotify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BackImage = global::EspionSpotify.Properties.Resources.spytify_logo;
            this.BackImagePadding = new System.Windows.Forms.Padding(24, 12, 0, 0);
            this.BackMaxSize = 250;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.lnkSpy);
            this.Controls.Add(this.tcMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FrmEspionSpotify";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Style = MetroFramework.MetroColorStyle.Green;
            this.Theme = MetroFramework.MetroThemeStyle.Default;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmEspionSpotify_FormClosing);
            this.tcMenu.ResumeLayout(false);
            this.tabRecord.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel12.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            this.tableLayoutPanel13.ResumeLayout(false);
            this.tableLayoutPanel13.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private MetroTabControl tcMenu;
        private MetroTabPage tabRecord;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private MetroLabel lblPath;
        private MetroLabel lblCustomize;
        private MetroLabel lblMinLength;
        private MetroLabel lblFormat;
        private MetroLabel lblBitRate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private MetroTrackBar tbMinTime;
        private MetroLabel lblMinTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private MetroTextBox txtPath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel12;
        private MetroComboBox cbBitRate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel13;
        private MetroRadioButton rbWav;
        private MetroRadioButton rbMp3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private MetroLabel lblAddFolders;
        private MetroToggle tgAddFolders;
        private MetroLabel lblAddSeparators;
        private MetroToggle tgAddSeparators;
        private MetroLabel lblNumFiles;
        private MetroToggle tgNumFiles;
        private MetroLabel lblNumTracks;
        private MetroToggle tgNumTracks;
        private MetroLabel lblRecordingNum;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private MetroLabel lblNum;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private MetroLabel lblSoundCard;
        private MetroLabel lblVolume;
        private MetroTrackBar tbVolumeWin;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private MetroPanel iconVolume;
        private MetroLink lnkDirectory;
        private MetroLink lnkClear;
        private MetroLink lnkNumPlus;
        private MetroLink lnkNumMinus;
        private MetroLink lnkPath;
        private MetroLink lnkSpy;
        private MetroFramework.Components.MetroToolTip tip;
        private MetroLabel lblPlayingTitle;
        private System.Windows.Forms.Panel iconSpotify;
        private MetroLabel lblLanguage;
        private MetroComboBox cbLanguage;
        private MetroToggle tgDisableAds;
        private MetroLabel lblAds;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private MetroLabel lblDisableAds;
        private System.Windows.Forms.Timer timer1;
    }
}

