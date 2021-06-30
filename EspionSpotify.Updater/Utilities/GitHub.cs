using EspionSpotify.Updater.Models.GitHub;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EspionSpotify.Updater.Utilities
{
    internal class GitHub
    {
        internal const string API_LATEST_RELEASES_LINK = "https://api.github.com/repos/jwallet/spy-spotify/releases";
        internal const string LATEST_RELEASE_LINK = "https://github.com/jwallet/spy-spotify/releases/latest";

        internal async static Task<Release[]> GetReleases()
        {
            Console.WriteLine("Getting missing releases...");

            var content = await Web.SendWebRequestAndGetContentAsync(API_LATEST_RELEASES_LINK);
            var releases = JsonConvert.DeserializeObject<Release[]>(content);

            if (releases == null)
            {
                Console.WriteLine("Releases were not found.");
                return null;
            }

            var relatedReleases = releases.Reverse().Where(r => AssemblyInfo.IsNewerVersionThanCurrent(r.tag_name)).ToArray();

            if (!relatedReleases.Any())
            {
                Console.WriteLine("You are already using the latest version.");
                Updater.LeaveConsole();
            }

            return relatedReleases;
        }

        internal static Asset GetZipAssetFromRelease(Release release)
        {
            var asset = release.assets
                .Where(a => Regex.IsMatch(a.name, @"^Spytify(-|\.)(v?)(\.)?\d+(\.\d+)?(\.\d+)?.zip$"))
                .FirstOrDefault();

            if (asset == null)
            {
                Console.WriteLine("This Release did not have any suitable asset to download. Try again later.");
                throw new ReleaseAssetNotFoundException();
            }
            else
            {
                Console.WriteLine("Release asset found: {0}", asset.name);
            }

            return asset;
        }
    }
}
