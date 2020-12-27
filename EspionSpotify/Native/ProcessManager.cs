using EspionSpotify.Native.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Native
{
    public class ProcessManager: IProcessManager
    {
        public IProcess GetCurrentProcess()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            return new Process
            {
                Id = process.Id,
                MainWindowTitle = process.MainWindowTitle,
                ProcessName = process.ProcessName
            };
        }

        public IProcess[] GetProcesses()
        {
            var processes = System.Diagnostics.Process.GetProcesses();
            return processes.Select(x => new Process
            {
                Id = x.Id,
                MainWindowTitle = x.MainWindowTitle,
                ProcessName = x.ProcessName
            }).ToArray();
        }

        public IProcess[] GetProcessesByName(string processName)
        {
            var processes = System.Diagnostics.Process.GetProcessesByName(processName);
            return processes.Select(x => new Process
            {
                Id = x.Id,
                MainWindowTitle = x.MainWindowTitle,
                ProcessName = x.ProcessName
            }).ToArray();
        }

        public IProcess GetProcessById(int processId)
        {
            var process = System.Diagnostics.Process.GetProcessById(processId);
            return new Process
            {
                Id = process.Id,
                MainWindowTitle = process.MainWindowTitle,
                ProcessName = process.ProcessName
            };
        }

        public IProcess Start(string fileName)
        {
            var process = System.Diagnostics.Process.Start(fileName);
            return new Process
            {
                Id = process.Id,
                MainWindowTitle = process.MainWindowTitle,
                ProcessName = process.ProcessName
            };
        }
    }
}
