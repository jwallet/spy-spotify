using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Extensions
{
    public static class ClassPropertyExtensions
    {
        public static void CopyAllTo<TSource, TTarget>(this TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            var parentProperties = source.GetType().GetProperties();
            var childProperties = target.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                foreach (var childProperty in childProperties)
                {
                    if (!childProperty.CanWrite || !parentProperty.CanRead) break;
                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                    {   
                        childProperty.SetValue(target, parentProperty.GetValue(source));
                        break;
                    }
                }
            }
        }
    }
}
