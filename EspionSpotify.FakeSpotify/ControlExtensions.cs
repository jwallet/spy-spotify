using System;
using System.Windows.Forms;

namespace EspionSpotify.FakeSpotify
{
    public static class ControlExtensions
    {
        public static TResult GetPropertyThreadSafe<TControl, TResult>(this TControl control,
            Func<TControl, TResult> getter)
            where TControl : Control
        {
            if (control.InvokeRequired)
                return (TResult) control.Invoke(getter, control);
            return getter(control);
        }

        public static void SetPropertyThreadSafe<TControl>(this TControl control, MethodInvoker setter)
            where TControl : Control
        {
            lock (control)
            {
                if (control.InvokeRequired)
                    control.Invoke(setter);
                else
                    setter();
            }
        }
    }
}