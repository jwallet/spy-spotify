using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Extensions
{
    public static class LinqExtensions
    {
        public static double Median(this IEnumerable<double> source)
        {
            if (source.Count() == 0)
            {
                throw new InvalidOperationException("Cannot compute median for an empty set.");
            }

            var sortedList = from number in source orderby number select number;

            int itemIndex = (int)sortedList.Count() / 2;

            if (sortedList.Count() % 2 == 0)
            {
                return (sortedList.ElementAt(itemIndex) + sortedList.ElementAt(itemIndex - 1)) / 2;
            }
            else
            {
                return sortedList.ElementAt(itemIndex);
            }
        }

        public static double Median(this IEnumerable<int> source)
        {
            return (from num in source select (double)num).Median();
        }

        public static double Median<T>(this IEnumerable<T> numbers, Func<T, double> selector)
        {
            return (from num in numbers select selector(num)).Median();
        }
    }
}
