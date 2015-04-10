using System.Collections.Generic;

namespace Simple.Wpf.Exceptions.Extensions
{
    public static class AddRangeExtensions
    {
        public static void AddRange<T>(this ICollection<T> oc, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                oc.Add(item);
            }
        }
    }
}
