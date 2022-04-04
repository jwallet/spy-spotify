using System.Threading.Tasks;
using EspionSpotify.Models;

namespace EspionSpotify.API
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