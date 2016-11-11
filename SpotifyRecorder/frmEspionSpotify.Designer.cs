namespace EspionSpotify
{
    partial class frmEspionSpotify
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEspionSpotify));
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.directoryButton = new System.Windows.Forms.Button();
            this.directoryLabel = new System.Windows.Forms.Label();
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.cbAddFolders = new System.Windows.Forms.CheckBox();
            this.cbAddSeparator = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.bitrateComboBox = new System.Windows.Forms.ComboBox();
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.clearButton = new System.Windows.Forms.Button();
            this.dirButton = new System.Windows.Forms.Button();
            this.recordButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbVolumeWin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinTime)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
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
            // statusTextBox
            // 
            this.statusTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusTextBox.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusTextBox.Location = new System.Drawing.Point(3, 133);
            this.statusTextBox.Multiline = true;
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ReadOnly = true;
            this.statusTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.statusTextBox.Size = new System.Drawing.Size(678, 276);
            this.statusTextBox.TabIndex = 25;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel8, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel10, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel2, 0, 3);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 4;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(678, 124);
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
            this.tableLayoutPanel7.Controls.Add(this.directoryButton, 2, 0);
            this.tableLayoutPanel7.Controls.Add(this.directoryLabel, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.directoryTextBox, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.cbAddFolders, 3, 0);
            this.tableLayoutPanel7.Controls.Add(this.cbAddSeparator, 4, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(672, 29);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // directoryButton
            // 
            this.directoryButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directoryButton.Location = new System.Drawing.Point(334, 4);
            this.directoryButton.Name = "directoryButton";
            this.directoryButton.Size = new System.Drawing.Size(56, 21);
            this.directoryButton.TabIndex = 27;
            this.directoryButton.Text = "←";
            this.directoryButton.UseVisualStyleBackColor = true;
            this.directoryButton.Click += new System.EventHandler(this.directoryButton_Click);
            // 
            // directoryLabel
            // 
            this.directoryLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.directoryLabel.AutoSize = true;
            this.directoryLabel.Location = new System.Drawing.Point(4, 8);
            this.directoryLabel.Name = "directoryLabel";
            this.directoryLabel.Size = new System.Drawing.Size(119, 13);
            this.directoryLabel.TabIndex = 1;
            this.directoryLabel.Text = "Dossier de sauvegarde:";
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.directoryTextBox.Location = new System.Drawing.Point(135, 4);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.Size = new System.Drawing.Size(192, 20);
            this.directoryTextBox.TabIndex = 26;
            // 
            // cbAddFolders
            // 
            this.cbAddFolders.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbAddFolders.AutoSize = true;
            this.cbAddFolders.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbAddFolders.Location = new System.Drawing.Point(397, 6);
            this.cbAddFolders.Name = "cbAddFolders";
            this.cbAddFolders.Size = new System.Drawing.Size(142, 17);
            this.cbAddFolders.TabIndex = 28;
            this.cbAddFolders.Text = "Structuration en dossiers";
            this.cbAddFolders.UseVisualStyleBackColor = true;
            this.cbAddFolders.CheckedChanged += new System.EventHandler(this.cbAddFolders_CheckedChanged);
            // 
            // cbAddSeparator
            // 
            this.cbAddSeparator.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbAddSeparator.AutoSize = true;
            this.cbAddSeparator.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbAddSeparator.Checked = true;
            this.cbAddSeparator.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAddSeparator.Location = new System.Drawing.Point(546, 6);
            this.cbAddSeparator.Name = "cbAddSeparator";
            this.cbAddSeparator.Size = new System.Drawing.Size(122, 17);
            this.cbAddSeparator.TabIndex = 29;
            this.cbAddSeparator.Text = "Retirer_les_espaces";
            this.cbAddSeparator.UseVisualStyleBackColor = true;
            this.cbAddSeparator.CheckedChanged += new System.EventHandler(this.cbAddSeparator_CheckedChanged);
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
            this.tableLayoutPanel8.Controls.Add(this.bitrateComboBox, 4, 0);
            this.tableLayoutPanel8.Controls.Add(this.outputLabel, 3, 0);
            this.tableLayoutPanel8.Controls.Add(this.rbWav, 2, 0);
            this.tableLayoutPanel8.Controls.Add(this.formatLabel, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.rbMp3, 1, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 38);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(672, 29);
            this.tableLayoutPanel8.TabIndex = 1;
            // 
            // bitrateComboBox
            // 
            this.bitrateComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.bitrateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bitrateComboBox.FormattingEnabled = true;
            this.bitrateComboBox.Location = new System.Drawing.Point(442, 4);
            this.bitrateComboBox.Name = "bitrateComboBox";
            this.bitrateComboBox.Size = new System.Drawing.Size(226, 21);
            this.bitrateComboBox.TabIndex = 3;
            // 
            // outputLabel
            // 
            this.outputLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(321, 8);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(111, 13);
            this.outputLabel.TabIndex = 3;
            this.outputLabel.Text = "Qualité audio (Bitrate):";
            // 
            // rbWav
            // 
            this.rbWav.AutoSize = true;
            this.rbWav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbWav.Location = new System.Drawing.Point(196, 4);
            this.rbWav.Name = "rbWav";
            this.rbWav.Size = new System.Drawing.Size(118, 21);
            this.rbWav.TabIndex = 10;
            this.rbWav.Tag = "wav";
            this.rbWav.Text = "WAV";
            this.rbWav.UseVisualStyleBackColor = true;
            this.rbWav.CheckedChanged += new System.EventHandler(this.rbFormat_CheckedChanged);
            // 
            // formatLabel
            // 
            this.formatLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.formatLabel.AutoSize = true;
            this.formatLabel.Location = new System.Drawing.Point(4, 8);
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Size = new System.Drawing.Size(122, 13);
            this.formatLabel.TabIndex = 8;
            this.formatLabel.Text = "Format d\'enregistrement:";
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
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 73);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(672, 29);
            this.tableLayoutPanel10.TabIndex = 3;
            // 
            // lblSoundCard
            // 
            this.lblSoundCard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblSoundCard.AutoSize = true;
            this.lblSoundCard.Location = new System.Drawing.Point(600, 8);
            this.lblSoundCard.Name = "lblSoundCard";
            this.lblSoundCard.Size = new System.Drawing.Size(67, 13);
            this.lblSoundCard.TabIndex = 28;
            this.lblSoundCard.Text = "Carte de son";
            // 
            // minTimelabel
            // 
            this.minTimelabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.minTimelabel.AutoSize = true;
            this.minTimelabel.Location = new System.Drawing.Point(4, 8);
            this.minTimelabel.Name = "minTimelabel";
            this.minTimelabel.Size = new System.Drawing.Size(82, 13);
            this.minTimelabel.TabIndex = 9;
            this.minTimelabel.Text = "Durée minimale:";
            // 
            // tbVolumeWin
            // 
            this.tbVolumeWin.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbVolumeWin.Location = new System.Drawing.Point(420, 4);
            this.tbVolumeWin.Maximum = 20;
            this.tbVolumeWin.Name = "tbVolumeWin";
            this.tbVolumeWin.Size = new System.Drawing.Size(173, 21);
            this.tbVolumeWin.TabIndex = 21;
            this.tbVolumeWin.Tag = "";
            this.tbVolumeWin.Scroll += new System.EventHandler(this.tbVolumeWin_Scroll);
            // 
            // volumelabel
            // 
            this.volumelabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.volumelabel.AutoSize = true;
            this.volumelabel.Location = new System.Drawing.Point(283, 8);
            this.volumelabel.Name = "volumelabel";
            this.volumelabel.Size = new System.Drawing.Size(87, 13);
            this.volumelabel.TabIndex = 10;
            this.volumelabel.Text = "Volume principal:";
            // 
            // lblVolume
            // 
            this.lblVolume.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(379, 8);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(33, 13);
            this.lblVolume.TabIndex = 9;
            this.lblVolume.Text = "100%";
            // 
            // tbMinTime
            // 
            this.tbMinTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMinTime.Cursor = System.Windows.Forms.Cursors.Default;
            this.tbMinTime.Location = new System.Drawing.Point(136, 4);
            this.tbMinTime.Maximum = 12;
            this.tbMinTime.Name = "tbMinTime";
            this.tbMinTime.Size = new System.Drawing.Size(140, 21);
            this.tbMinTime.TabIndex = 14;
            this.tbMinTime.Value = 3;
            this.tbMinTime.Scroll += new System.EventHandler(this.tbMinTime_Scroll);
            // 
            // lblMinTime
            // 
            this.lblMinTime.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblMinTime.AutoSize = true;
            this.lblMinTime.Location = new System.Drawing.Point(98, 8);
            this.lblMinTime.Name = "lblMinTime";
            this.lblMinTime.Size = new System.Drawing.Size(28, 13);
            this.lblMinTime.TabIndex = 15;
            this.lblMinTime.Text = "0:30";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.currentlyPlayingLabel, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 108);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(672, 16);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(177, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Présentement en lecture sur Spotify:";
            // 
            // currentlyPlayingLabel
            // 
            this.currentlyPlayingLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.currentlyPlayingLabel.AutoSize = true;
            this.currentlyPlayingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentlyPlayingLabel.Location = new System.Drawing.Point(186, 1);
            this.currentlyPlayingLabel.Name = "currentlyPlayingLabel";
            this.currentlyPlayingLabel.Size = new System.Drawing.Size(0, 13);
            this.currentlyPlayingLabel.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel6, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.statusTextBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel11, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(684, 462);
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
            this.tableLayoutPanel11.Location = new System.Drawing.Point(3, 415);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 1;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(678, 44);
            this.tableLayoutPanel11.TabIndex = 26;
            // 
            // clearButton
            // 
            this.clearButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clearButton.Location = new System.Drawing.Point(228, 3);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(219, 38);
            this.clearButton.TabIndex = 23;
            this.clearButton.Text = "Effacer l\'historique";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // dirButton
            // 
            this.dirButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dirButton.Location = new System.Drawing.Point(453, 3);
            this.dirButton.Name = "dirButton";
            this.dirButton.Size = new System.Drawing.Size(222, 38);
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
            this.recordButton.Size = new System.Drawing.Size(219, 38);
            this.recordButton.TabIndex = 18;
            this.recordButton.Text = "Débuter l\'écoute";
            this.recordButton.UseVisualStyleBackColor = true;
            this.recordButton.Click += new System.EventHandler(this.recordButton_Click);
            // 
            // frmEspionSpotify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 462);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "frmEspionSpotify";
            this.Text = "Espion Spotify";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpotifyRecorderForm_FormClosing);
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
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel11.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox statusTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label lblSoundCard;
        private System.Windows.Forms.Label volumelabel;
        private System.Windows.Forms.TrackBar tbVolumeWin;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Button directoryButton;
        private System.Windows.Forms.Label directoryLabel;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.ComboBox bitrateComboBox;
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
        private System.Windows.Forms.CheckBox cbAddFolders;
        private System.Windows.Forms.CheckBox cbAddSeparator;
    }
}

