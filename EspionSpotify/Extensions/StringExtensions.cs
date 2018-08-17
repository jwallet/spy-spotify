using EspionSpotify.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Extensions
{
    public static class StringExtensions
    {
        public static AlbumCoverSize? ConvertToAlbumCoverSize(this string value)
        {
            var types = typeof(AlbumCoverSize);

            if (string.IsNullOrEmpty(value) || !Enum.IsDefined(types, value))
            {
                return null;
            }

            return (AlbumCoverSize) Enum.Parse(types, value, ignoreCase: true);
        }
    }
}
