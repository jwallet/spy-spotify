using MetroFramework.Controls;
using MetroFramework.Forms;

namespace EspionSpotify.FakeSpotify
{
    partial class FrmSpotify
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Artist - Track");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Artist - Track - Live In Town");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Artist - Track (feat. DJ)");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Advertisement");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSpotify));
            this.tbVolume = new MetroFramework.Controls.MetroTrackBar();
            this.btnPlayback = new MetroFramework.Controls.MetroButton();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.chkLockWindowTitleToPlaybackState = new MetroFramework.Controls.MetroCheckBox();
            this.lblWindowTitle = new MetroFramework.Controls.MetroLabel();
            this.txtWindowTitle = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.lstPlaylist = new MetroFramework.Controls.MetroListView();
            this.btnDeleteInPlaylist = new MetroFramework.Controls.MetroButton();
            this.txtAddToPlaylist = new MetroFramework.Controls.MetroTextBox();
            this.btnAddToPlaylist = new MetroFramework.Controls.MetroButton();
            this.btnNextTrack = new MetroFramework.Controls.MetroButton();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.tbDelayTitle = new MetroFramework.Controls.MetroTrackBar();
            this.lblDelayTitle = new MetroFramework.Controls.MetroLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbVolume
            // 
            this.tbVolume.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbVolume.BackColor = System.Drawing.Color.Transparent;
            this.tbVolume.Location = new System.Drawing.Point(61, 5);
            this.tbVolume.Name = "tbVolume";
            this.tbVolume.Size = new System.Drawing.Size(310, 23);
            this.tbVolume.Style = MetroFramework.MetroColorStyle.Green;
            this.tbVolume.TabIndex = 2;
            this.tbVolume.Text = "Volume";
            this.tbVolume.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbVolume.Value = 100;
            this.tbVolume.Scroll += new System.Windows.Forms.ScrollEventHandler(this.tbVolume_Scroll);
            // 
            // btnPlayback
            // 
            this.btnPlayback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPlayback.Location = new System.Drawing.Point(3, 3);
            this.btnPlayback.Name = "btnPlayback";
            this.btnPlayback.Size = new System.Drawing.Size(181, 28);
            this.btnPlayback.Style = MetroFramework.MetroColorStyle.Green;
            this.btnPlayback.TabIndex = 3;
            this.btnPlayback.Text = "Play";
            this.btnPlayback.Theme = MetroFramework.MetroThemeStyle.Light;
            this.btnPlayback.UseSelectable = true;
            this.btnPlayback.Click += new System.EventHandler(this.btnPlayback_Click);
            // 
            // metroLabel1
            // 
            this.metroLabel1.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel1.Location = new System.Drawing.Point(0, 7);
            this.metroLabel1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(55, 19);
            this.metroLabel1.Style = MetroFramework.MetroColorStyle.Green;
            this.metroLabel1.TabIndex = 4;
            this.metroLabel1.Text = "Volume";
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // chkLockWindowTitleToPlaybackState
            // 
            this.chkLockWindowTitleToPlaybackState.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkLockWindowTitleToPlaybackState.Checked = true;
            this.chkLockWindowTitleToPlaybackState.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLockWindowTitleToPlaybackState.FontWeight = MetroFramework.MetroCheckBoxWeight.Bold;
            this.chkLockWindowTitleToPlaybackState.Location = new System.Drawing.Point(6, 118);
            this.chkLockWindowTitleToPlaybackState.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.chkLockWindowTitleToPlaybackState.Name = "chkLockWindowTitleToPlaybackState";
            this.chkLockWindowTitleToPlaybackState.Size = new System.Drawing.Size(368, 24);
            this.chkLockWindowTitleToPlaybackState.Style = MetroFramework.MetroColorStyle.Green;
            this.chkLockWindowTitleToPlaybackState.TabIndex = 5;
            this.chkLockWindowTitleToPlaybackState.Text = "Lock Window Title To Playback State";
            this.chkLockWindowTitleToPlaybackState.Theme = MetroFramework.MetroThemeStyle.Light;
            this.chkLockWindowTitleToPlaybackState.UseSelectable = true;
            this.chkLockWindowTitleToPlaybackState.CheckedChanged += new System.EventHandler(this.chkLockWindowTitleToPlaybackState_CheckedChanged);
            // 
            // lblWindowTitle
            // 
            this.lblWindowTitle.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWindowTitle.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lblWindowTitle.ForeColor = System.Drawing.Color.Coral;
            this.lblWindowTitle.Location = new System.Drawing.Point(3, 7);
            this.lblWindowTitle.Name = "lblWindowTitle";
            this.lblWindowTitle.Size = new System.Drawing.Size(374, 23);
            this.lblWindowTitle.Style = MetroFramework.MetroColorStyle.Green;
            this.lblWindowTitle.TabIndex = 6;
            this.lblWindowTitle.Text = "Window Title";
            this.lblWindowTitle.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // txtWindowTitle
            // 
            this.txtWindowTitle.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.txtWindowTitle.CustomButton.Image = null;
            this.txtWindowTitle.CustomButton.Location = new System.Drawing.Point(346, 1);
            this.txtWindowTitle.CustomButton.Name = "";
            this.txtWindowTitle.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.txtWindowTitle.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtWindowTitle.CustomButton.TabIndex = 1;
            this.txtWindowTitle.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtWindowTitle.CustomButton.UseSelectable = true;
            this.txtWindowTitle.CustomButton.Visible = false;
            this.txtWindowTitle.Enabled = false;
            this.txtWindowTitle.FontWeight = MetroFramework.MetroTextBoxWeight.Bold;
            this.txtWindowTitle.Lines = new string[] {"Spotify"};
            this.txtWindowTitle.Location = new System.Drawing.Point(6, 33);
            this.txtWindowTitle.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.txtWindowTitle.MaxLength = 32767;
            this.txtWindowTitle.Name = "txtWindowTitle";
            this.txtWindowTitle.PasswordChar = '\0';
            this.txtWindowTitle.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtWindowTitle.SelectedText = "";
            this.txtWindowTitle.SelectionLength = 0;
            this.txtWindowTitle.SelectionStart = 0;
            this.txtWindowTitle.ShortcutsEnabled = true;
            this.txtWindowTitle.Size = new System.Drawing.Size(368, 23);
            this.txtWindowTitle.Style = MetroFramework.MetroColorStyle.Green;
            this.txtWindowTitle.TabIndex = 7;
            this.txtWindowTitle.Text = "Spotify";
            this.txtWindowTitle.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtWindowTitle.UseSelectable = true;
            this.txtWindowTitle.WaterMarkColor = System.Drawing.Color.FromArgb(((int) (((byte) (109)))), ((int) (((byte) (109)))), ((int) (((byte) (109)))));
            this.txtWindowTitle.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.txtWindowTitle.Leave += new System.EventHandler(this.txtWindowTitle_Leave);
            // 
            // metroLabel3
            // 
            this.metroLabel3.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.metroLabel3.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel3.Location = new System.Drawing.Point(3, 5);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(287, 23);
            this.metroLabel3.Style = MetroFramework.MetroColorStyle.Green;
            this.metroLabel3.TabIndex = 8;
            this.metroLabel3.Text = "Playlist";
            this.metroLabel3.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // lstPlaylist
            // 
            this.lstPlaylist.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.lstPlaylist.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstPlaylist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstPlaylist.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lstPlaylist.FullRowSelect = true;
            this.lstPlaylist.HideSelection = false;
            listViewItem1.StateImageIndex = 0;
            listViewItem2.StateImageIndex = 0;
            listViewItem4.StateImageIndex = 0;
            this.lstPlaylist.Items.AddRange(new System.Windows.Forms.ListViewItem[] {listViewItem1, listViewItem2, listViewItem3, listViewItem4});
            this.lstPlaylist.LabelEdit = true;
            this.lstPlaylist.Location = new System.Drawing.Point(9, 43);
            this.lstPlaylist.Margin = new System.Windows.Forms.Padding(9, 3, 6, 3);
            this.lstPlaylist.Name = "lstPlaylist";
            this.lstPlaylist.OwnerDraw = true;
            this.lstPlaylist.Size = new System.Drawing.Size(365, 148);
            this.lstPlaylist.Style = MetroFramework.MetroColorStyle.Green;
            this.lstPlaylist.TabIndex = 9;
            this.lstPlaylist.Theme = MetroFramework.MetroThemeStyle.Light;
            this.lstPlaylist.UseCompatibleStateImageBehavior = false;
            this.lstPlaylist.UseSelectable = true;
            this.lstPlaylist.View = System.Windows.Forms.View.List;
            this.lstPlaylist.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lstPlaylist_ItemSelectionChanged);
            this.lstPlaylist.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstPlaylist_MouseDoubleClick);
            // 
            // btnDeleteInPlaylist
            // 
            this.btnDeleteInPlaylist.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnDeleteInPlaylist.AutoSize = true;
            this.btnDeleteInPlaylist.Enabled = false;
            this.btnDeleteInPlaylist.Location = new System.Drawing.Point(296, 5);
            this.btnDeleteInPlaylist.Name = "btnDeleteInPlaylist";
            this.btnDeleteInPlaylist.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteInPlaylist.Style = MetroFramework.MetroColorStyle.Green;
            this.btnDeleteInPlaylist.TabIndex = 10;
            this.btnDeleteInPlaylist.Text = "Delete";
            this.btnDeleteInPlaylist.Theme = MetroFramework.MetroThemeStyle.Light;
            this.btnDeleteInPlaylist.UseSelectable = true;
            this.btnDeleteInPlaylist.Click += new System.EventHandler(this.btnDeleteInPlaylist_Click);
            // 
            // txtAddToPlaylist
            // 
            this.txtAddToPlaylist.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.txtAddToPlaylist.CustomButton.Image = null;
            this.txtAddToPlaylist.CustomButton.Location = new System.Drawing.Point(268, 1);
            this.txtAddToPlaylist.CustomButton.Name = "";
            this.txtAddToPlaylist.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.txtAddToPlaylist.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtAddToPlaylist.CustomButton.TabIndex = 1;
            this.txtAddToPlaylist.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtAddToPlaylist.CustomButton.UseSelectable = true;
            this.txtAddToPlaylist.CustomButton.Visible = false;
            this.txtAddToPlaylist.FontWeight = MetroFramework.MetroTextBoxWeight.Bold;
            this.txtAddToPlaylist.Lines = new string[0];
            this.txtAddToPlaylist.Location = new System.Drawing.Point(3, 5);
            this.txtAddToPlaylist.MaxLength = 32767;
            this.txtAddToPlaylist.Name = "txtAddToPlaylist";
            this.txtAddToPlaylist.PasswordChar = '\0';
            this.txtAddToPlaylist.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtAddToPlaylist.SelectedText = "";
            this.txtAddToPlaylist.SelectionLength = 0;
            this.txtAddToPlaylist.SelectionStart = 0;
            this.txtAddToPlaylist.ShortcutsEnabled = true;
            this.txtAddToPlaylist.Size = new System.Drawing.Size(290, 23);
            this.txtAddToPlaylist.Style = MetroFramework.MetroColorStyle.Green;
            this.txtAddToPlaylist.TabIndex = 11;
            this.txtAddToPlaylist.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtAddToPlaylist.UseSelectable = true;
            this.txtAddToPlaylist.WaterMarkColor = System.Drawing.Color.FromArgb(((int) (((byte) (109)))), ((int) (((byte) (109)))), ((int) (((byte) (109)))));
            this.txtAddToPlaylist.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // btnAddToPlaylist
            // 
            this.btnAddToPlaylist.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnAddToPlaylist.AutoSize = true;
            this.btnAddToPlaylist.Location = new System.Drawing.Point(299, 5);
            this.btnAddToPlaylist.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.btnAddToPlaylist.Name = "btnAddToPlaylist";
            this.btnAddToPlaylist.Size = new System.Drawing.Size(75, 23);
            this.btnAddToPlaylist.Style = MetroFramework.MetroColorStyle.Green;
            this.btnAddToPlaylist.TabIndex = 12;
            this.btnAddToPlaylist.Text = "Add";
            this.btnAddToPlaylist.Theme = MetroFramework.MetroThemeStyle.Light;
            this.btnAddToPlaylist.UseSelectable = true;
            this.btnAddToPlaylist.Click += new System.EventHandler(this.btnAddToPlaylist_Click);
            // 
            // btnNextTrack
            // 
            this.btnNextTrack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNextTrack.Location = new System.Drawing.Point(190, 3);
            this.btnNextTrack.Name = "btnNextTrack";
            this.btnNextTrack.Size = new System.Drawing.Size(181, 28);
            this.btnNextTrack.Style = MetroFramework.MetroColorStyle.Green;
            this.btnNextTrack.TabIndex = 13;
            this.btnNextTrack.Text = "Next Track";
            this.btnNextTrack.Theme = MetroFramework.MetroThemeStyle.Light;
            this.btnNextTrack.UseSelectable = true;
            this.btnNextTrack.Click += new System.EventHandler(this.btnNextTrack_Click);
            // 
            // metroLabel4
            // 
            this.metroLabel4.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel4.Location = new System.Drawing.Point(0, 7);
            this.metroLabel4.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(72, 19);
            this.metroLabel4.Style = MetroFramework.MetroColorStyle.Green;
            this.metroLabel4.TabIndex = 14;
            this.metroLabel4.Text = "Delay Title";
            this.metroLabel4.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // tbDelayTitle
            // 
            this.tbDelayTitle.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDelayTitle.BackColor = System.Drawing.Color.Transparent;
            this.tbDelayTitle.LargeChange = 1000;
            this.tbDelayTitle.Location = new System.Drawing.Point(78, 5);
            this.tbDelayTitle.Maximum = 10000;
            this.tbDelayTitle.Name = "tbDelayTitle";
            this.tbDelayTitle.Size = new System.Drawing.Size(234, 23);
            this.tbDelayTitle.SmallChange = 100;
            this.tbDelayTitle.Style = MetroFramework.MetroColorStyle.Green;
            this.tbDelayTitle.TabIndex = 15;
            this.tbDelayTitle.Text = "Delay";
            this.tbDelayTitle.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbDelayTitle.Value = 0;
            this.tbDelayTitle.Scroll += new System.Windows.Forms.ScrollEventHandler(this.tbDelayTitle_Scroll);
            // 
            // lblDelayTitle
            // 
            this.lblDelayTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDelayTitle.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblDelayTitle.Location = new System.Drawing.Point(318, 5);
            this.lblDelayTitle.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblDelayTitle.Name = "lblDelayTitle";
            this.lblDelayTitle.Size = new System.Drawing.Size(56, 23);
            this.lblDelayTitle.Style = MetroFramework.MetroColorStyle.Green;
            this.lblDelayTitle.TabIndex = 16;
            this.lblDelayTitle.Text = "0ms";
            this.lblDelayTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDelayTitle.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(20, 30);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(772, 259);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lstPlaylist, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(389, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(380, 253);
            this.tableLayoutPanel2.TabIndex = 18;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.metroLabel3, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnDeleteInPlaylist, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(374, 34);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.txtAddToPlaylist, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnAddToPlaylist, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 197);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(374, 34);
            this.tableLayoutPanel4.TabIndex = 10;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.lblWindowTitle, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.txtWindowTitle, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel6, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.chkLockWindowTitleToPlaybackState, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel7, 0, 4);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel8, 0, 5);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 7;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(380, 253);
            this.tableLayoutPanel5.TabIndex = 19;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 3;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.Controls.Add(this.metroLabel4, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tbDelayTitle, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.lblDelayTitle, 2, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 73);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(374, 34);
            this.tableLayoutPanel6.TabIndex = 8;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Controls.Add(this.metroLabel1, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.tbVolume, 1, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 153);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(374, 34);
            this.tableLayoutPanel7.TabIndex = 9;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Controls.Add(this.btnPlayback, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnNextTrack, 1, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 193);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(374, 34);
            this.tableLayoutPanel8.TabIndex = 10;
            // 
            // FrmSpotify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(812, 309);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "FrmSpotify";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Text = "Spotify";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        private MetroFramework.Controls.MetroButton btnNextTrack;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroTrackBar tbDelayTitle;
        private MetroFramework.Controls.MetroLabel lblDelayTitle;

        private MetroFramework.Controls.MetroTextBox txtAddToPlaylist;
        private MetroFramework.Controls.MetroButton btnAddToPlaylist;

        private MetroFramework.Controls.MetroLabel lblWindowTitle;
        private MetroFramework.Controls.MetroTextBox txtWindowTitle;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroListView lstPlaylist;
        private MetroFramework.Controls.MetroButton btnDeleteInPlaylist;

        private MetroFramework.Controls.MetroTrackBar tbVolume;
        private MetroFramework.Controls.MetroButton btnPlayback;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroCheckBox chkLockWindowTitleToPlaybackState;

        #endregion
    }
}