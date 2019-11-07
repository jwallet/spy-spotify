using EspionSpotify.Enums;
using EspionSpotify.Extensions;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace EspionSpotify.Ads
{
    internal class Administrator
    {
        public static bool EnsureAdmin()
        {
            if (!IsNotAdmin()) return true;
            if (MetroFramework.MetroMessageBox.Show(
                FrmEspionSpotify.Instance,
                FrmEspionSpotify.Rm.GetString(TranslationKeys.msgEnsureAdminContent),
                FrmEspionSpotify.Rm.GetString(TranslationKeys.msgEnsureAdminTitle),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes) return false;

            var exe = Process.GetCurrentProcess().MainModule.FileName;
            var info = new ProcessStartInfo(exe)
            {
                Verb = "runas",
                UseShellExecute = true
            };

            try
            {
                Process.Start(info);
                Application.Exit();
            }
            catch
            {
                return false;
            }

            return false;
        }

        private static bool IsNotAdmin()
        {
            var principalIdentity = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return !principalIdentity.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
