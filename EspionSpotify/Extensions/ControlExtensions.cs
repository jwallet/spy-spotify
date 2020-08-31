using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EspionSpotify.Extensions
{
    public static class ControlExtensions
    {
        public static Result GetThreadSafeValue<T, Result>(this T control, Expression<Func<T, Result>> selector)
            where T : Control
        {
            Func<T, Result> func = selector.Compile();

            if (control.InvokeRequired)
            {
                return (Result)control.Invoke(new Func<Result>(() => func(control)));
            }
            else
            {
                return func(control);
            }
        }
    }
}
