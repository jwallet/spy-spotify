using EspionSpotify.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Extensions
{
    public static class ResourceManagerExtensions
    {
        public static string GetString(this ResourceManager rm, TranslationKeys key)
        {
            return rm.GetString(key.ToString());
        }
    }
}
