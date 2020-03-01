using System.Threading.Tasks;

namespace EspionSpotify
{
    public interface IRecorder
    {
        int CountSeconds { get; set; }
        bool Running { get; set; }

        Task Run();
    }
}