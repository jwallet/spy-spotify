using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IProcess = EspionSpotify.Native.Models.IProcess;

namespace EspionSpotify.Native
{
    public interface IProcessManager
    {
        IProcess GetCurrentProcess();
        IProcess[] GetProcesses();
        IProcess[] GetProcessesByName(string processName);
        IProcess GetProcessById(int processId);

        IProcess Start(string fileName);
    }
}
