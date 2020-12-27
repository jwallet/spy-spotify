namespace EspionSpotify.Native.Models
{
    public class Process : IProcess
    {
        public int Id { get; set; }
        public string MainWindowTitle { get; set; }
        public string ProcessName { get; set; }
    }
}
