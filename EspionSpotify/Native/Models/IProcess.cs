using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Native.Models
{
    public interface IProcess
    {
        int Id { get; }
        string MainWindowTitle { get; }
        string ProcessName { get; }
    }
}
