namespace EspionSpotify
{
    public interface IRecorder
    {
        int CountSeconds { get; set; }
        bool Running { get; set; }

        void Run();
    }
}