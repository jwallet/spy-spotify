using System;

namespace EspionSpotify.Native.Models
{
    public interface IProcess
    {
        int Id { get; }
        string MainWindowTitle { get; }
        string ProcessName { get; }
        IntPtr MainWindowHandle { get; }
    }
}