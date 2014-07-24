using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Helpers
{
    public static class HashSetExtensions
    {
        public static HashSet<T> AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                hashSet.Add(item);
            }
            return hashSet;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> collection)
        {
            return new HashSet<T>(collection);
        }

        public static HashSet<T> FindAll<T>(this HashSet<T> collection, Func<T, bool> predicate)
        {
            return new HashSet<T>(collection.Where(predicate));
        }
    }
}