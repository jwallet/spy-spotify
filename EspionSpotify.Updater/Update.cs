using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ionic.Zip;
using MetroFramework.Forms;

namespace EspionSpotify.Updater
{
    public partial class Update : MetroForm
    {
        public Update(string[] args)
        {

            InitializeComponent();
            Thread t = new Thread(() => {
                ProcessUpdate(args);
            });
            t.Start();

        }
        public void ProcessUpdate(string[] args)
        {
            if (args.Length==0)
            {
                MessageBox.Show("An error occurred while updating. Please try it again later","Update failed",MessageBoxButtons.OK,MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            string path = args[0];
            try
            {
                extract(path);
            }
            catch( Exception ex)
            {
                MessageBox.Show("An error occurred while updating. Please try it again later"+ ex.Message, "Update failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo();
            p.StartInfo.FileName= path+"Spytify.exe";
            p.StartInfo.Arguments = "upgradeSettings";
            p.Start();
            Environment.Exit(0);
        }
        public void extract(string path)
        {
            ZipFile zip = ZipFile.Read(path+"update.zip");
            zip.ExtractAll(path,ExtractExistingFileAction.OverwriteSilently);
            zip.Dispose();
            File.Delete(path + "update.zip");
        }
    }
}
