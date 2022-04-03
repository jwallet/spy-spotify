using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EspionSpotify.Extensions;
using EspionSpotify.Properties;
using EspionSpotify.Translations;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Controls;
using MetroFramework.Forms;

namespace EspionSpotify.Controls
{
    public class FrmSpotifyAPICredentials : MetroForm
    {
        private const int OAUTH2_KEY_LENGTH = 32;
        private readonly Analytics _analytics;

        private readonly string _pastSpotifyAPIClientId;
        private readonly string _pastSpotifyAPIRedirectURL;
        private readonly string _pastSpotifyAPISecretId;
        private MetroLabel lblClientId;
        private MetroLabel lblRedirectURL;
        private MetroLabel lblSecretId;
        private MetroLink lnkFAQSpotifyAPI;
        private MetroLink lnkSpotifyAPIDashboard;
        private TableLayoutPanel tableLayoutPanel2;
        private MetroToolTip tip;
        private MetroTextBox txtClientId;
        private MetroTextBox txtRedirectURL;
        private MetroTextBox txtSecretId;

        public FrmSpotifyAPICredentials(Analytics analytics)
        {
            InitializeComponent();

            _analytics = analytics;

            _pastSpotifyAPIClientId = Settings.Default.app_spotify_api_client_id?.Trim();
            _pastSpotifyAPISecretId = Settings.Default.app_spotify_api_client_secret?.Trim();
            _pastSpotifyAPIRedirectURL = Settings.Default.app_spotify_api_redirect_url?.Trim();

            if (string.IsNullOrEmpty(_pastSpotifyAPIRedirectURL))
                _pastSpotifyAPIRedirectURL = API.SpotifyAPI.SPOTIFY_API_DEFAULT_REDIRECT_URL;

            txtClientId.Text = _pastSpotifyAPIClientId;
            txtSecretId.Text = _pastSpotifyAPISecretId;
            txtRedirectURL.Text = _pastSpotifyAPIRedirectURL;

            Text = FrmEspionSpotify.Instance.Rm.GetString(I18NKeys.TitleSpotifyAPICredentials);

            lblClientId.Text = FrmEspionSpotify.Instance.Rm.GetString(I18NKeys.LblClientId);
            lblSecretId.Text = FrmEspionSpotify.Instance.Rm.GetString(I18NKeys.LblSecretId);
            lblRedirectURL.Text = FrmEspionSpotify.Instance.Rm.GetString(I18NKeys.LblRedirectURL);

            txtClientId.WaterMark = FrmEspionSpotify.Instance.Rm.GetString(I18NKeys.WatermarkClientId);
            txtSecretId.WaterMark = FrmEspionSpotify.Instance.Rm.GetString(I18NKeys.WatermarkSecretId);
            txtRedirectURL.WaterMark = FrmEspionSpotify.Instance.Rm.GetString(I18NKeys.WatermarkRedirectURL);

            tip.SetToolTip(lnkFAQSpotifyAPI, FrmEspionSpotify.Instance.Rm.GetString(I18NKeys.TipFAQSpotifyAPI));
            tip.SetToolTip(lnkSpotifyAPIDashboard,
                FrmEspionSpotify.Instance.Rm.GetString(I18NKeys.TipSpotifyAPIDashboard));
        }

        #region Components

        private void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof(FrmSpotifyAPICredentials));
            tableLayoutPanel2 = new TableLayoutPanel();
            txtRedirectURL = new MetroTextBox();
            txtClientId = new MetroTextBox();
            txtSecretId = new MetroTextBox();
            lblClientId = new MetroLabel();
            lblSecretId = new MetroLabel();
            lblRedirectURL = new MetroLabel();
            lnkFAQSpotifyAPI = new MetroLink();
            tip = new MetroToolTip();
            lnkSpotifyAPIDashboard = new MetroLink();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(txtRedirectURL, 1, 2);
            tableLayoutPanel2.Controls.Add(txtClientId, 1, 0);
            tableLayoutPanel2.Controls.Add(txtSecretId, 1, 1);
            tableLayoutPanel2.Controls.Add(lblClientId, 0, 0);
            tableLayoutPanel2.Controls.Add(lblSecretId, 0, 1);
            tableLayoutPanel2.Controls.Add(lblRedirectURL, 0, 2);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(10, 60);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.Padding = new Padding(0, 10, 0, 0);
            tableLayoutPanel2.RowCount = 4;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.Size = new Size(400, 122);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // txtRedirectURL
            // 
            txtRedirectURL.BackColor = Color.Black;
            // 
            // 
            // 
            txtRedirectURL.CustomButton.Image = null;
            txtRedirectURL.CustomButton.Location = new Point(238, 1);
            txtRedirectURL.CustomButton.Name = "";
            txtRedirectURL.CustomButton.Size = new Size(27, 27);
            txtRedirectURL.CustomButton.Style = MetroColorStyle.Blue;
            txtRedirectURL.CustomButton.TabIndex = 1;
            txtRedirectURL.CustomButton.Theme = MetroThemeStyle.Light;
            txtRedirectURL.CustomButton.UseSelectable = true;
            txtRedirectURL.CustomButton.Visible = false;
            txtRedirectURL.Dock = DockStyle.Fill;
            txtRedirectURL.FontSize = MetroTextBoxSize.Medium;
            txtRedirectURL.ForeColor = Color.FromArgb(224, 224, 224);
            txtRedirectURL.Lines = new string[0];
            txtRedirectURL.Location = new Point(131, 83);
            txtRedirectURL.MaxLength = 32;
            txtRedirectURL.Name = "txtRedirectURL";
            txtRedirectURL.PasswordChar = '\0';
            txtRedirectURL.PromptText = "PASTE_REDIRECT_URL_HERE";
            txtRedirectURL.ScrollBars = ScrollBars.None;
            txtRedirectURL.SelectedText = "";
            txtRedirectURL.SelectionLength = 0;
            txtRedirectURL.SelectionStart = 0;
            txtRedirectURL.ShortcutsEnabled = true;
            txtRedirectURL.Size = new Size(266, 29);
            txtRedirectURL.Style = MetroColorStyle.Green;
            txtRedirectURL.TabIndex = 40;
            txtRedirectURL.TextAlign = HorizontalAlignment.Center;
            txtRedirectURL.Theme = MetroThemeStyle.Dark;
            txtRedirectURL.UseCustomBackColor = true;
            txtRedirectURL.UseSelectable = true;
            txtRedirectURL.WaterMark = "PASTE_REDIRECT_URL_HERE";
            txtRedirectURL.WaterMarkColor = Color.FromArgb(109, 109, 109);
            txtRedirectURL.WaterMarkFont = new Font("Segoe UI", 9.75F, FontStyle.Italic, GraphicsUnit.Point, 0);
            txtRedirectURL.TextChanged += TxtRedirectURL_TextChanged;
            // 
            // txtClientId
            // 
            txtClientId.BackColor = Color.Black;
            // 
            // 
            // 
            txtClientId.CustomButton.Image = null;
            txtClientId.CustomButton.Location = new Point(238, 1);
            txtClientId.CustomButton.Name = "";
            txtClientId.CustomButton.Size = new Size(27, 27);
            txtClientId.CustomButton.Style = MetroColorStyle.Blue;
            txtClientId.CustomButton.TabIndex = 1;
            txtClientId.CustomButton.Theme = MetroThemeStyle.Light;
            txtClientId.CustomButton.UseSelectable = true;
            txtClientId.CustomButton.Visible = false;
            txtClientId.Dock = DockStyle.Fill;
            txtClientId.FontSize = MetroTextBoxSize.Medium;
            txtClientId.ForeColor = Color.FromArgb(224, 224, 224);
            txtClientId.Lines = new string[0];
            txtClientId.Location = new Point(131, 13);
            txtClientId.MaxLength = 32;
            txtClientId.Name = "txtClientId";
            txtClientId.PasswordChar = '\0';
            txtClientId.PromptText = "PASTE_KEY_ID_HERE";
            txtClientId.ScrollBars = ScrollBars.None;
            txtClientId.SelectedText = "";
            txtClientId.SelectionLength = 0;
            txtClientId.SelectionStart = 0;
            txtClientId.ShortcutsEnabled = true;
            txtClientId.Size = new Size(266, 29);
            txtClientId.Style = MetroColorStyle.Green;
            txtClientId.TabIndex = 35;
            txtClientId.TextAlign = HorizontalAlignment.Center;
            txtClientId.Theme = MetroThemeStyle.Dark;
            txtClientId.UseCustomBackColor = true;
            txtClientId.UseSelectable = true;
            txtClientId.WaterMark = "PASTE_KEY_ID_HERE";
            txtClientId.WaterMarkColor = Color.FromArgb(109, 109, 109);
            txtClientId.WaterMarkFont = new Font("Segoe UI", 9.75F, FontStyle.Italic, GraphicsUnit.Point, 0);
            txtClientId.TextChanged += TxtClientId_TextChanged;
            // 
            // txtSecretId
            // 
            txtSecretId.BackColor = Color.Black;
            // 
            // 
            // 
            txtSecretId.CustomButton.Image = null;
            txtSecretId.CustomButton.Location = new Point(238, 1);
            txtSecretId.CustomButton.Name = "";
            txtSecretId.CustomButton.Size = new Size(27, 27);
            txtSecretId.CustomButton.Style = MetroColorStyle.Blue;
            txtSecretId.CustomButton.TabIndex = 1;
            txtSecretId.CustomButton.Theme = MetroThemeStyle.Light;
            txtSecretId.CustomButton.UseSelectable = true;
            txtSecretId.CustomButton.Visible = false;
            txtSecretId.Dock = DockStyle.Fill;
            txtSecretId.FontSize = MetroTextBoxSize.Medium;
            txtSecretId.ForeColor = Color.FromArgb(224, 224, 224);
            txtSecretId.Lines = new string[0];
            txtSecretId.Location = new Point(131, 48);
            txtSecretId.MaxLength = 32;
            txtSecretId.Name = "txtSecretId";
            txtSecretId.PasswordChar = '\0';
            txtSecretId.PromptText = "PASTE_KEY_ID_HERE";
            txtSecretId.ScrollBars = ScrollBars.None;
            txtSecretId.SelectedText = "";
            txtSecretId.SelectionLength = 0;
            txtSecretId.SelectionStart = 0;
            txtSecretId.ShortcutsEnabled = true;
            txtSecretId.Size = new Size(266, 29);
            txtSecretId.Style = MetroColorStyle.Green;
            txtSecretId.TabIndex = 36;
            txtSecretId.TextAlign = HorizontalAlignment.Center;
            txtSecretId.Theme = MetroThemeStyle.Dark;
            txtSecretId.UseCustomBackColor = true;
            txtSecretId.UseSelectable = true;
            txtSecretId.WaterMark = "PASTE_KEY_ID_HERE";
            txtSecretId.WaterMarkColor = Color.FromArgb(109, 109, 109);
            txtSecretId.WaterMarkFont = new Font("Segoe UI", 9.75F, FontStyle.Italic, GraphicsUnit.Point, 0);
            txtSecretId.TextChanged += TxtSecretId_TextChanged;
            // 
            // lblClientId
            // 
            lblClientId.AutoSize = true;
            lblClientId.Dock = DockStyle.Fill;
            lblClientId.ForeColor = Color.FromArgb(224, 224, 224);
            lblClientId.Location = new Point(3, 10);
            lblClientId.Name = "lblClientId";
            lblClientId.Size = new Size(122, 35);
            lblClientId.TabIndex = 37;
            lblClientId.Text = "LBL_CLIENT_ID";
            lblClientId.TextAlign = ContentAlignment.MiddleRight;
            lblClientId.Theme = MetroThemeStyle.Dark;
            // 
            // lblSecretId
            // 
            lblSecretId.AutoSize = true;
            lblSecretId.Dock = DockStyle.Fill;
            lblSecretId.ForeColor = Color.FromArgb(224, 224, 224);
            lblSecretId.Location = new Point(3, 45);
            lblSecretId.Name = "lblSecretId";
            lblSecretId.Size = new Size(122, 35);
            lblSecretId.TabIndex = 38;
            lblSecretId.Text = "LBL_SECRET_ID";
            lblSecretId.TextAlign = ContentAlignment.MiddleRight;
            lblSecretId.Theme = MetroThemeStyle.Dark;
            // 
            // lblRedirectURL
            // 
            lblRedirectURL.AutoSize = true;
            lblRedirectURL.Dock = DockStyle.Fill;
            lblRedirectURL.ForeColor = Color.FromArgb(224, 224, 224);
            lblRedirectURL.Location = new Point(3, 80);
            lblRedirectURL.Name = "lblRedirectURL";
            lblRedirectURL.Size = new Size(122, 35);
            lblRedirectURL.TabIndex = 39;
            lblRedirectURL.Text = "LBL_REDIRECT_URL";
            lblRedirectURL.TextAlign = ContentAlignment.MiddleRight;
            lblRedirectURL.Theme = MetroThemeStyle.Dark;
            // 
            // lnkFAQSpotifyAPI
            // 
            lnkFAQSpotifyAPI.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lnkFAQSpotifyAPI.Image = Resources.faq;
            lnkFAQSpotifyAPI.ImageSize = 12;
            lnkFAQSpotifyAPI.Location = new Point(369, 5);
            lnkFAQSpotifyAPI.Margin = new Padding(0);
            lnkFAQSpotifyAPI.Name = "lnkFAQSpotifyAPI";
            lnkFAQSpotifyAPI.Size = new Size(18, 18);
            lnkFAQSpotifyAPI.TabIndex = 44;
            lnkFAQSpotifyAPI.UseCustomBackColor = true;
            lnkFAQSpotifyAPI.UseSelectable = true;
            lnkFAQSpotifyAPI.Click += LnkFAQSpotifyAPI_Click;
            // 
            // tip
            // 
            tip.Style = MetroColorStyle.Default;
            tip.StyleManager = null;
            tip.Theme = MetroThemeStyle.Light;
            // 
            // lnkSpotifyAPIDashboard
            // 
            lnkSpotifyAPIDashboard.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lnkSpotifyAPIDashboard.Image = Resources.spotify;
            lnkSpotifyAPIDashboard.ImageSize = 14;
            lnkSpotifyAPIDashboard.Location = new Point(344, 5);
            lnkSpotifyAPIDashboard.Margin = new Padding(0);
            lnkSpotifyAPIDashboard.Name = "lnkSpotifyAPIDashboard";
            lnkSpotifyAPIDashboard.Size = new Size(18, 18);
            lnkSpotifyAPIDashboard.TabIndex = 45;
            lnkSpotifyAPIDashboard.UseCustomBackColor = true;
            lnkSpotifyAPIDashboard.UseSelectable = true;
            lnkSpotifyAPIDashboard.Click += LnkSpotifyAPIDashboard_Click;
            // 
            // FrmSpotifyAPICredentials
            // 
            BackgroundImageLayout = ImageLayout.None;
            BorderStyle = MetroFormBorderStyle.FixedSingle;
            ClientSize = new Size(420, 202);
            Controls.Add(lnkSpotifyAPIDashboard);
            Controls.Add(lnkFAQSpotifyAPI);
            Controls.Add(tableLayoutPanel2);
            Icon = (Icon) resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Movable = false;
            Name = "FrmSpotifyAPICredentials";
            Padding = new Padding(10, 60, 10, 20);
            Resizable = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Style = MetroColorStyle.Green;
            Text = "SPOTIFY_API_CREDENTIALS";
            Theme = MetroThemeStyle.Dark;
            FormClosing += FrmSpotifyAPICredentials_FormClosing;
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion Components

        private void TxtClientId_TextChanged(object sender, EventArgs e)
        {
            var value = ((MetroTextBox) sender).Text.Trim();
            if (!string.IsNullOrEmpty(value) && value.Length != OAUTH2_KEY_LENGTH) return;

            Settings.Default.app_spotify_api_client_id = value;
            Settings.Default.Save();
        }

        private void TxtSecretId_TextChanged(object sender, EventArgs e)
        {
            var value = ((MetroTextBox) sender).Text.Trim();
            if (!string.IsNullOrEmpty(value) && value.Length != OAUTH2_KEY_LENGTH) return;

            Settings.Default.app_spotify_api_client_secret = value;
            Settings.Default.Save();
        }

        private void FrmSpotifyAPICredentials_FormClosing(object sender, FormClosingEventArgs e)
        {
            var haveIds = !string.IsNullOrEmpty(txtClientId.Text.Trim()) &&
                          !string.IsNullOrEmpty(txtSecretId.Text.Trim());

            var redirectURL = string.IsNullOrWhiteSpace(txtRedirectURL.Text)
                ? API.SpotifyAPI.SPOTIFY_API_DEFAULT_REDIRECT_URL
                : txtRedirectURL.Text;

            var sameClientId = _pastSpotifyAPIClientId == txtClientId.Text;
            var sameSecretId = _pastSpotifyAPISecretId == txtSecretId.Text;
            var sameRedirectURL = _pastSpotifyAPIRedirectURL == redirectURL;

            var allValuesSame = sameClientId && sameSecretId && sameRedirectURL;

            if (allValuesSame) DialogResult = DialogResult.Cancel;
            else if (haveIds) DialogResult = DialogResult.Yes;
            else DialogResult = DialogResult.No;
        }

        private void LnkFAQSpotifyAPI_Click(object sender, EventArgs e)
        {
            Process.Start(GitHub.WEBSITE_FAQ_SPOTIFY_API_URL);
            Task.Run(async () => await _analytics.LogAction("faq-spotify-api"));
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

        private void LnkSpotifyAPIDashboard_Click(object sender, EventArgs e)
        {
            Process.Start(API.SpotifyAPI.SPOTIFY_API_DASHBOARD_URL);
            Task.Run(async () => await _analytics.LogAction("spotify-api-dashboard"));
        }

        private void TxtRedirectURL_TextChanged(object sender, EventArgs e)
        {
            var value = ((MetroTextBox) sender).Text.Trim();

            Settings.Default.app_spotify_api_redirect_url = string.IsNullOrEmpty(value)
                ? API.SpotifyAPI.SPOTIFY_API_DEFAULT_REDIRECT_URL
                : value;
            Settings.Default.Save();
        }
    }
}