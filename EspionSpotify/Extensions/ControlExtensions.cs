using System;
using System.Windows.Forms;

namespace EspionSpotify.Extensions
{
    public static class ControlExtensions
    {
        public static TResult GetPropertyThreadSafe<TControl, TResult>(this TControl control,
            Func<TControl, TResult> getter)
            where TControl : Control
        {
            if (control.IsDisposed()) return default;
            if (control.InvokeRequired)
                return (TResult) control.Invoke(getter, control);
            return getter(control);
        }

        public static void SetPropertyThreadSafe<TControl>(this TControl control, MethodInvoker setter)
            where TControl : Control
        {
            lock (control)
            {
                if (control.IsDisposed()) return;
                if (control.InvokeRequired)
                    control.Invoke(setter);
                else
                    setter();
            }
        }

        public static bool IsDisposed<TControl>(this TControl control)
            where TControl : Control
        {
            return FrmEspionSpotify.Instance.IsDisposed || control.IsDisposed || control.Parent.IsDisposed;
        }
    }
}