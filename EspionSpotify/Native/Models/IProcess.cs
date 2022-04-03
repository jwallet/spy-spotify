namespace EspionSpotify.Native.Models
{
    public interface IProcess
    {
        int Id { get; }
        string MainWindowTitle { get; }
        string ProcessName { get; }
    }
}