using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EspionSpotify.Models
{
    internal class RecorderTask
    {
        public Task Task { get; set; }
        public CancellationTokenSource Token { get; set; }
    }
}
