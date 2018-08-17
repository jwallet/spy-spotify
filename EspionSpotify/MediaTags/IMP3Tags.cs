using EspionSpotify.Models;
using System.Threading.Tasks;

namespace EspionSpotify.MediaTags
{
    public interface IMP3Tags
    {
        string CurrentFile { get; set; }
        int? Count { get; set; }
        bool OrderNumberInMediaTagEnabled { get; set; }
        Track Track { get; set; }

        Task SaveMediaTags();
    }
}
