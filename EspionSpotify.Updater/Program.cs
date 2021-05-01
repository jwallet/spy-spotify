using EspionSpotify.Updater.GitHub;
using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EspionSpotify.Updater
{
    class Program
    {
        static void Main()
        {
            Thread t = new Thread(() =>
            {
                ProcessUpdate();
            });
            t.Start();
        }
        public static void ProcessUpdate()
        {
            Console.WriteLine("Downloading update...");
            if (!downloadUpdate())
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                Environment.Exit(1);
            }
            try
            {
                Console.WriteLine("Extracting");
                extract(AppDomain.CurrentDomain.BaseDirectory);
            }
            catch (System.UnauthorizedAccessException ex)
            {
                Console.WriteLine("An error occurred while extracting. Make sure Spytify is not running or Antivirus Software is interferring.\n"+
                    "You can extract the file manually at: "+ AppDomain.CurrentDomain.BaseDirectory+"../update.zip\n"+
                    "Error Message: "+ ex.Message);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                Environment.Exit(1);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occurred while extracting\n" +
                    "The error was" + ex.Message);
                Console.ReadKey();
            }
            File.Delete(AppDomain.CurrentDomain.BaseDirectory + "../update.zip");
            Console.WriteLine("Update successfull");
            Process.Start(new ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + "../Spytify.exe", "upgradeSettings"));
            Environment.Exit(0);
        }
        public static bool isVersionNewer(string name)
        {
            Match m = Regex.Match(name, @"v\.((?:\d+\.)*\d+?).zip$");
            if (m.Groups[1].Success)
            {
                string currentVersion = null;
                try
                {
                    currentVersion = FileVersionInfo.GetVersionInfo(AppDomain.CurrentDomain.BaseDirectory + "../Spytify.exe").FileVersion;
                }
                catch
                {
                    return true;
                }
                
                if (currentVersion.StartsWith(m.Groups[1].Value))
                    return false;
                return true;
            }
            return true;
        }
        public static bool downloadUpdate()
        {
            try
            {
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/jwallet/spy-spotify/releases/latest");
                wr.Method = WebRequestMethods.Http.Get;
                wr.UserAgent = "Spytify";
                WebResponse ws = wr.GetResponse();
                string json = new StreamReader(ws.GetResponseStream()).ReadToEnd();
                Release release = JsonConvert.DeserializeObject<Release>(json);
                Asset targetRelease = null;
                foreach (Asset asset in release.assets)
                {
                    if (Regex.IsMatch(asset.name, @"v\.((?:\d+\.)*\d+?).zip$"))
                    {
                        targetRelease = asset;
                        break;
                    }
                }
                if (targetRelease == null)
                {
                    Console.WriteLine("This Release did not have any suitable asset to download\n");
                    return false;
                }
                if (!isVersionNewer(targetRelease.name))
                {
                    Console.WriteLine("You are already using the latest version");
                    return false;
                }
                WebClient wc = new WebClient();
                wc.DownloadFile(targetRelease.browser_download_url, AppDomain.CurrentDomain.BaseDirectory+ "../update.zip");
                wc.Dispose();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Something went wrong during the download. Make sure it was not blocked by your Antivirus Software\n" +
                    "The error was: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong during the download. The error was: \n" + ex.Message);
                return false;
            }
            return true;
        }
        public static void extract(string path)
        {
            ZipFile zip = ZipFile.Read(path + "../update.zip");

            zip.ExtractAll(path+"../", ExtractExistingFileAction.OverwriteSilently);
            zip.Dispose();      
        }
    }
}
