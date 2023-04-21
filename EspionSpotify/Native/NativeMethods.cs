using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace EspionSpotify.Native
{
    internal static class NativeMethods
    {
        internal static void PreventSleep()
        {
            SetThreadExecutionState(ExecutionState.EsContinuous | ExecutionState.EsSystemRequired);
        }

        internal static void AllowSleep()
        {
            SetThreadExecutionState(ExecutionState.EsContinuous);
        }

        internal static void SendKeyPessNextMedia(IntPtr process)
        {
            SendKeyPessMedia(process, SpotifyAction.NextTrack);
        }

        internal static void SendKeyPressPauseMedia(IntPtr process)
        {
            SendKeyPessMedia(process, SpotifyAction.PlayPause);
        }

        private static void SendKeyPessMedia(IntPtr process, SpotifyAction action)
        {
            Task.Run(() =>
            {
                SendMessage(process, 0x0319, IntPtr.Zero, new IntPtr((long)action));
            });
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        
        [FlagsAttribute]
        private enum ExecutionState : uint
        {
            EsAwaymodeRequired = 0x00000040,
            EsContinuous = 0x80000000,
            EsDisplayRequired = 0x00000002,
            EsSystemRequired = 0x00000001
        }

        private enum SpotifyAction : long
        {
            PlayPause = 917504,
            Mute = 524288,
            VolumeDown = 589824,
            VolumeUp = 655360,
            Stop = 851968,
            PreviousTrack = 786432,
            NextTrack = 720896
        }
    }
}