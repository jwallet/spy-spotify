namespace EspionSpotify
{
    partial class FrmEspionSpotify
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.chkAddFolders = new System.Windows.Forms.CheckBox();
            this.btnPath = new System.Windows.Forms.Button();
            this.directoryLabel = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.chkAddSeparator = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.cbBitRate = new System.Windows.Forms.ComboBox();
            this.outputLabel = new System.Windows.Forms.Label();
            this.rbWav = new System.Windows.Forms.RadioButton();
            this.formatLabel = new System.Windows.Forms.Label();
            this.rbMp3 = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.lblSoundCard = new System.Windows.Forms.Label();
            this.minTimelabel = new System.Windows.Forms.Label();
            this.tbVolumeWin = new System.Windows.Forms.TrackBar();
            this.volumelabel = new System.Windows.Forms.Label();
            this.lblVolume = new System.Windows.Forms.Label();
            this.tbMinTime = new System.Windows.Forms.TrackBar();
            this.lblMinTime = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.currentlyPlayingLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.chkCdNums = new System.Windows.Forms.CheckBox();
            this.chkNumFile = new System.Windows.Forms.CheckBox();
            this.numTrack = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.clearButton = new System.Windows.Forms.Button();
            this.dirButton = new System.Windows.Forms.Button();
            this.recordButton = new System.Windows.Forms.Button();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbVolumeWin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinTime)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTrack)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.SuspendLayout();
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Veuillez sélectionner l\'emplacement où vous souhaitez sauvegarder les fichiers au" +
    "dio.";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel8, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel10, 0, 3);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel2, 0, 4);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 5;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(790, 162);
            this.tableLayoutPanel6.TabIndex = 27;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel7.ColumnCount = 5;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.Controls.Add(this.chkAddFolders, 3, 0);
            this.tableLayoutPanel7.Controls.Add(this.btnPath, 2, 0);
            this.tableLayoutPanel7.Controls.Add(this.directoryLabel, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.txtPath, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.chkAddSeparator, 4, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(784, 29);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // chkAddFolders
            // 
            this.chkAddFolders.AutoSize = true;
            this.chkAddFolders.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAddFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkAddFolders.Location = new System.Drawing.Point(575, 4);
            this.chkAddFolders.Name = "chkAddFolders";
            this.chkAddFolders.Size = new System.Drawing.Size(102, 21);
            this.chkAddFolders.TabIndex = 32;
            this.chkAddFolders.Text = "Artiste = Dossier";
            this.chkAddFolders.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAddFolders.UseVisualStyleBackColor = true;
            this.chkAddFolders.CheckedChanged += new System.EventHandler(this.cbAddFolders_CheckedChanged);
            // 
            // btnPath
            // 
            this.btnPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPath.Location = new System.Drawing.Point(512, 4);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(56, 21);
            this.btnPath.TabIndex = 27;
            this.btnPath.Text = "←";
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.directoryButton_Click);
            // 
            // directoryLabel
            // 
            this.directoryLabel.AutoSize = true;
            this.directoryLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directoryLabel.Location = new System.Drawing.Point(4, 1);
            this.directoryLabel.Name = "directoryLabel";
            this.directoryLabel.Size = new System.Drawing.Size(124, 27);
            this.directoryLabel.TabIndex = 1;
            this.directoryLabel.Text = "Dossier de sauvegarde:";
            this.directoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(135, 4);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(370, 20);
            this.txtPath.TabIndex = 26;
            this.txtPath.DoubleClick += new System.EventHandler(this.directoryButton_Click);
            // 
            // chkAddSeparator
            // 
            this.chkAddSeparator.AutoSize = true;
            this.chkAddSeparator.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAddSeparator.Checked = true;
            this.chkAddSeparator.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAddSeparator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkAddSeparator.Location = new System.Drawing.Point(684, 4);
            this.chkAddSeparator.Name = "chkAddSeparator";
            this.chkAddSeparator.Size = new System.Drawing.Size(96, 21);
            this.chkAddSeparator.TabIndex = 29;
            this.chkAddSeparator.Text = "Sans_espaces";
            this.chkAddSeparator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAddSeparator.UseVisualStyleBackColor = true;
            this.chkAddSeparator.CheckedChanged += new System.EventHandler(this.cbAddSeparator_CheckedChanged);
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel8.ColumnCount = 5;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel8.Controls.Add(this.cbBitRate, 4, 0);
            this.tableLayoutPanel8.Controls.Add(this.outputLabel, 3, 0);
            this.tableLayoutPanel8.Controls.Add(this.rbWav, 2, 0);
            this.tableLayoutPanel8.Controls.Add(this.formatLabel, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.rbMp3, 1, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 73);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(784, 29);
            this.tableLayoutPanel8.TabIndex = 1;
            // 
            // cbBitRate
            // 
            this.cbBitRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBitRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBitRate.FormattingEnabled = true;
            this.cbBitRate.Location = new System.Drawing.Point(481, 4);
            this.cbBitRate.Name = "cbBitRate";
            this.cbBitRate.Size = new System.Drawing.Size(299, 21);
            this.cbBitRate.TabIndex = 3;
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputLabel.Location = new System.Drawing.Point(360, 1);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(114, 27);
            this.outputLabel.TabIndex = 3;
            this.outputLabel.Text = "Qualité audio (Bitrate):";
            this.outputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rbWav
            // 
            this.rbWav.AutoSize = true;
            this.rbWav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbWav.Location = new System.Drawing.Point(196, 4);
            this.rbWav.Name = "rbWav";
            this.rbWav.Size = new System.Drawing.Size(157, 21);
            this.rbWav.TabIndex = 10;
            this.rbWav.Tag = "wav";
            this.rbWav.Text = "WAV";
            this.rbWav.UseVisualStyleBackColor = true;
            this.rbWav.CheckedChanged += new System.EventHandler(this.rbFormat_CheckedChanged);
            // 
            // formatLabel
            // 
            this.formatLabel.AutoSize = true;
            this.formatLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formatLabel.Location = new System.Drawing.Point(4, 1);
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Size = new System.Drawing.Size(124, 27);
            this.formatLabel.TabIndex = 8;
            this.formatLabel.Text = "Format d\'enregistrement:";
            this.formatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rbMp3
            // 
            this.rbMp3.AutoSize = true;
            this.rbMp3.Checked = true;
            this.rbMp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbMp3.Location = new System.Drawing.Point(135, 4);
            this.rbMp3.Name = "rbMp3";
            this.rbMp3.Size = new System.Drawing.Size(54, 21);
            this.rbMp3.TabIndex = 9;
            this.rbMp3.TabStop = true;
            this.rbMp3.Tag = "mp3";
            this.rbMp3.Text = "MP3";
            this.rbMp3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMp3.UseVisualStyleBackColor = true;
            this.rbMp3.CheckedChanged += new System.EventHandler(this.rbFormat_CheckedChanged);
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel10.ColumnCount = 7;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel10.Controls.Add(this.lblSoundCard, 6, 0);
            this.tableLayoutPanel10.Controls.Add(this.minTimelabel, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.tbVolumeWin, 5, 0);
            this.tableLayoutPanel10.Controls.Add(this.volumelabel, 3, 0);
            this.tableLayoutPanel10.Controls.Add(this.lblVolume, 4, 0);
            this.tableLayoutPanel10.Controls.Add(this.tbMinTime, 2, 0);
            this.tableLayoutPanel10.Controls.Add(this.lblMinTime, 1, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 108);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(784, 29);
            this.tableLayoutPanel10.TabIndex = 3;
            // 
            // lblSoundCard
            // 
            this.lblSoundCard.AutoSize = true;
            this.lblSoundCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSoundCard.Location = new System.Drawing.Point(712, 1);
            this.lblSoundCard.Name = "lblSoundCard";
            this.lblSoundCard.Size = new System.Drawing.Size(68, 27);
            this.lblSoundCard.TabIndex = 28;
            this.lblSoundCard.Text = "Carte de son";
            this.lblSoundCard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // minTimelabel
            // 
            this.minTimelabel.AutoSize = true;
            this.minTimelabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.minTimelabel.Location = new System.Drawing.Point(4, 1);
            this.minTimelabel.Name = "minTimelabel";
            this.minTimelabel.Size = new System.Drawing.Size(84, 27);
            this.minTimelabel.TabIndex = 9;
            this.minTimelabel.Text = "Durée minimale:";
            this.minTimelabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbVolumeWin
            // 
            this.tbVolumeWin.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbVolumeWin.Location = new System.Drawing.Point(471, 4);
            this.tbVolumeWin.Maximum = 20;
            this.tbVolumeWin.Name = "tbVolumeWin";
            this.tbVolumeWin.Size = new System.Drawing.Size(234, 21);
            this.tbVolumeWin.TabIndex = 21;
            this.tbVolumeWin.Tag = "";
            this.tbVolumeWin.Scroll += new System.EventHandler(this.tbVolumeWin_Scroll);
            // 
            // volumelabel
            // 
            this.volumelabel.AutoSize = true;
            this.volumelabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.volumelabel.Location = new System.Drawing.Point(334, 1);
            this.volumelabel.Name = "volumelabel";
            this.volumelabel.Size = new System.Drawing.Size(89, 27);
            this.volumelabel.TabIndex = 10;
            this.volumelabel.Text = "Volume principal:";
            this.volumelabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVolume
            // 
            this.lblVolume.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(430, 8);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(33, 13);
            this.lblVolume.TabIndex = 9;
            this.lblVolume.Text = "100%";
            // 
            // tbMinTime
            // 
            this.tbMinTime.Cursor = System.Windows.Forms.Cursors.Default;
            this.tbMinTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbMinTime.Location = new System.Drawing.Point(136, 4);
            this.tbMinTime.Maximum = 12;
            this.tbMinTime.Name = "tbMinTime";
            this.tbMinTime.Size = new System.Drawing.Size(191, 21);
            this.tbMinTime.TabIndex = 14;
            this.tbMinTime.Value = 3;
            this.tbMinTime.Scroll += new System.EventHandler(this.tbMinTime_Scroll);
            // 
            // lblMinTime
            // 
            this.lblMinTime.AutoSize = true;
            this.lblMinTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMinTime.Location = new System.Drawing.Point(95, 1);
            this.lblMinTime.Name = "lblMinTime";
            this.lblMinTime.Size = new System.Drawing.Size(34, 27);
            this.lblMinTime.TabIndex = 15;
            this.lblMinTime.Text = "0:30";
            this.lblMinTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.Black;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.currentlyPlayingLabel, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 140);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(790, 22);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(259, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "Présentement en lecture sur Spotify:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // currentlyPlayingLabel
            // 
            this.currentlyPlayingLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.currentlyPlayingLabel.AutoSize = true;
            this.currentlyPlayingLabel.BackColor = System.Drawing.Color.Black;
            this.currentlyPlayingLabel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentlyPlayingLabel.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.currentlyPlayingLabel.Location = new System.Drawing.Point(259, 4);
            this.currentlyPlayingLabel.Margin = new System.Windows.Forms.Padding(0);
            this.currentlyPlayingLabel.Name = "currentlyPlayingLabel";
            this.currentlyPlayingLabel.Size = new System.Drawing.Size(0, 14);
            this.currentlyPlayingLabel.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.chkCdNums, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.chkNumFile, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.numTrack, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 38);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(784, 29);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(4, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 27);
            this.label2.TabIndex = 47;
            this.label2.Text = "Débuter le compteur à:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkCdNums
            // 
            this.chkCdNums.AutoSize = true;
            this.chkCdNums.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCdNums.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkCdNums.Location = new System.Drawing.Point(431, 4);
            this.chkCdNums.Name = "chkCdNums";
            this.chkCdNums.Size = new System.Drawing.Size(227, 21);
            this.chkCdNums.TabIndex = 46;
            this.chkCdNums.Text = "Remplacer le No de Track par le compteur";
            this.chkCdNums.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCdNums.UseVisualStyleBackColor = true;
            this.chkCdNums.CheckedChanged += new System.EventHandler(this.chkCdNums_CheckedChanged);
            // 
            // chkNumFile
            // 
            this.chkNumFile.AutoSize = true;
            this.chkNumFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkNumFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkNumFile.Location = new System.Drawing.Point(193, 4);
            this.chkNumFile.Name = "chkNumFile";
            this.chkNumFile.Size = new System.Drawing.Size(231, 21);
            this.chkNumFile.TabIndex = 45;
            this.chkNumFile.Text = "Inscrire le compteur devant le titre du fichier";
            this.chkNumFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkNumFile.UseVisualStyleBackColor = true;
            this.chkNumFile.CheckedChanged += new System.EventHandler(this.chkNumFile_CheckedChanged);
            // 
            // numTrack
            // 
            this.numTrack.AutoSize = true;
            this.numTrack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numTrack.Location = new System.Drawing.Point(126, 4);
            this.numTrack.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numTrack.MaximumSize = new System.Drawing.Size(60, 0);
            this.numTrack.MinimumSize = new System.Drawing.Size(60, 0);
            this.numTrack.Name = "numTrack";
            this.numTrack.Size = new System.Drawing.Size(60, 20);
            this.numTrack.TabIndex = 43;
            this.numTrack.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTrack.ValueChanged += new System.EventHandler(this.numTrack_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel6, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel11, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.rtbLog, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(790, 517);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.ColumnCount = 3;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel11.Controls.Add(this.clearButton, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.dirButton, 2, 0);
            this.tableLayoutPanel11.Controls.Add(this.recordButton, 0, 0);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(3, 470);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 1;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(784, 44);
            this.tableLayoutPanel11.TabIndex = 26;
            // 
            // clearButton
            // 
            this.clearButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clearButton.Location = new System.Drawing.Point(264, 3);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(255, 38);
            this.clearButton.TabIndex = 23;
            this.clearButton.Text = "Effacer l\'historique";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // dirButton
            // 
            this.dirButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dirButton.Location = new System.Drawing.Point(525, 3);
            this.dirButton.Name = "dirButton";
            this.dirButton.Size = new System.Drawing.Size(256, 38);
            this.dirButton.TabIndex = 24;
            this.dirButton.Text = "Ouvrir le dossier de sauvegarde";
            this.dirButton.UseVisualStyleBackColor = true;
            this.dirButton.Click += new System.EventHandler(this.dirButton_Click);
            // 
            // recordButton
            // 
            this.recordButton.BackColor = System.Drawing.SystemColors.Control;
            this.recordButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.recordButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recordButton.Location = new System.Drawing.Point(3, 3);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(255, 38);
            this.recordButton.TabIndex = 18;
            this.recordButton.Text = "Débuter l\'écoute";
            this.recordButton.UseVisualStyleBackColor = true;
            this.recordButton.Click += new System.EventHandler(this.recordButton_Click);
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.Black;
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLog.ForeColor = System.Drawing.Color.Silver;
            this.rtbLog.Location = new System.Drawing.Point(0, 162);
            this.rtbLog.Margin = new System.Windows.Forms.Padding(0);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(790, 305);
            this.rtbLog.TabIndex = 28;
            this.rtbLog.Text = "";
            // 
            // FrmEspionSpotify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 517);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "FrmEspionSpotify";
            this.Text = "Espion Spotify";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmEspionSpotify_FormClosing);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbVolumeWin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinTime)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTrack)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel11.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label lblSoundCard;
        private System.Windows.Forms.Label volumelabel;
        private System.Windows.Forms.TrackBar tbVolumeWin;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label directoryLabel;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.ComboBox cbBitRate;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.RadioButton rbWav;
        private System.Windows.Forms.Label formatLabel;
        private System.Windows.Forms.RadioButton rbMp3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.Label minTimelabel;
        private System.Windows.Forms.TrackBar tbMinTime;
        private System.Windows.Forms.Label lblMinTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label currentlyPlayingLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel11;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button dirButton;
        private System.Windows.Forms.Button recordButton;
        private System.Windows.Forms.CheckBox chkAddSeparator;
        private System.Windows.Forms.CheckBox chkAddFolders;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.CheckBox chkNumFile;
        private System.Windows.Forms.NumericUpDown numTrack;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkCdNums;
        private System.Windows.Forms.RichTextBox rtbLog;
    }
}

