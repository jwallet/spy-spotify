using EspionSpotify.Models;
using NAudio.Lame;
using System.Threading.Tasks;

namespace EspionSpotify.MediaTags
{
    public interface IMapperID3
    {
        int? Count { get; set; }
        bool OrderNumberInMediaTagEnabled { get; set; }
        Track Track { get; set; }

        Task<ID3TagData> GetTags();
    }
}
