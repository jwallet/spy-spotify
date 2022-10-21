using System;
using System.Runtime.InteropServices;

namespace EspionSpotify.FakeSpotify
{
    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string text);

        public static void SetProcessMainWindow(IntPtr mainWindowHandler, string windowTitle)
        {
            SetWindowText(mainWindowHandler, windowTitle);
        }
    }
}