using System.Linq;
using EspionSpotify.Native.Models;
using NativeProcess = System.Diagnostics.Process;

namespace EspionSpotify.Native
{
    public class ProcessManager : IProcessManager
    {
        public IProcess GetCurrentProcess()
        {
            NativeProcess process;

            try
            {
                process = NativeProcess.GetCurrentProcess();
            }
            catch
            {
                return null;
            }

            return new Process
            {
                Id = process.Id,
                MainWindowTitle = process.MainWindowTitle,
                ProcessName = process.ProcessName
            };
        }

        public IProcess[] GetProcesses()
        {
            NativeProcess[] processes;

            try
            {
                processes = NativeProcess.GetProcesses();
            }
            catch
            {
                return new Process[] { };
            }

            return processes.Select(x => new Process
            {
                Id = x.Id,
                MainWindowTitle = x.MainWindowTitle,
                ProcessName = x.ProcessName
            }).ToArray();
        }

        public IProcess[] GetProcessesByName(string processName)
        {
            NativeProcess[] processes;

            try
            {
                processes = NativeProcess.GetProcessesByName(processName);
            }
            catch
            {
                return new Process[] { };
            }

            return processes.Select(x => new Process
            {
                Id = x.Id,
                MainWindowTitle = x.MainWindowTitle,
                ProcessName = x.ProcessName
            }).ToArray();
        }

        public IProcess GetProcessById(int processId)
        {
            NativeProcess process;
            
            try
            {
                process = NativeProcess.GetProcessById(processId);
            }
            catch
            {
                return null;
            }

            return new Process
            {
                Id = process.Id,
                MainWindowTitle = process.MainWindowTitle,
                ProcessName = process.ProcessName
            };
        }

        public IProcess Start(string fileName)
        {
            NativeProcess process;

            try
            {
                process = NativeProcess.Start(fileName);
            }
            catch
            {
                return null;
            }

            return new Process
            {
                Id = process.Id,
                MainWindowTitle = process.MainWindowTitle,
                ProcessName = process.ProcessName
            };
        }
    }
}
