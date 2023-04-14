using System;
using System.Collections.Generic;
using System.Linq;

namespace EspionSpotify.Extensions
{
    public static class LinqExtensions
    {
        public static T Median<T>(this IEnumerable<T> source)
        {
            var sourceList = source.ToList();
            if (!sourceList.Any()) return default;

            var sortedList = (from number in sourceList orderby number select number).ToList();

            var itemIndex = (int)(sortedList.Count() / 2);

            return sortedList.ElementAt(itemIndex);
        }

        public static double Median<T>(this IEnumerable<T> numbers, Func<T, double> selector)
        {
            var flatList = (from num in numbers select selector(num)).ToList();
            return flatList.Median<double>();
        }

        public static int Median<T>(this IEnumerable<T> numbers, Func<T, int> selector)
        {
            var flatList = (from num in numbers select selector(num)).ToList();
            return flatList.Median<int>();
        }

        // https://stackoverflow.com/questions/20469416/linq-to-find-series-of-consecutive-numbers
        public static IEnumerable<IEnumerable<T>> GroupWhile<T>(this IEnumerable<T> seq, Func<T, T, bool> condition)
        {
            T prev = seq.First();
            List<T> list = new List<T>() { prev };

            foreach (T item in seq.Skip(1))
            {
                if (condition(prev, item) == false)
                {
                    yield return list;
                    list = new List<T>();
                }
                list.Add(item);
                prev = item;
            }

            yield return list;
        }
    }
}