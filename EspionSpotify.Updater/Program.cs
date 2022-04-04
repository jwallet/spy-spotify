using System.Threading;

namespace EspionSpotify.Updater
{
    public class Program
    {
        internal static void Main()
        {
            var t = new Thread(Run);
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
            t.Join();
        }

        internal static void Run()
        {
            Updater.ProcessUpdateAsync().GetAwaiter().GetResult();
        }
    }
}