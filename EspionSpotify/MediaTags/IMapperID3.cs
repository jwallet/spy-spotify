using EspionSpotify.Models;
using System.Threading.Tasks;

namespace EspionSpotify.MediaTags
{
    public interface IMapperID3
    {
        string CurrentFile { get; set; }
        int? Count { get; set; }
        bool OrderNumberInMediaTagEnabled { get; set; }
        Track Track { get; set; }

        Task SaveMediaTags();
    }
}
