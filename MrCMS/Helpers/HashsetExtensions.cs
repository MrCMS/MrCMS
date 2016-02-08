using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Helpers
{
    public static class HashSetExtensions
    {
        public static ISet<T> AddRange<T>(this ISet<T> set, IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                set.Add(item);
            }
            return set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="equalityComparer">Use this to allow non-standard comparers for the set</param>
        /// <returns></returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> collection, IEqualityComparer<T> equalityComparer = null)
        {
            return equalityComparer != null
                ? new HashSet<T>(collection, equalityComparer)
                : new HashSet<T>(collection);
        }

        public static HashSet<T> FindAll<T>(this HashSet<T> collection, Func<T, bool> predicate)
        {
            return new HashSet<T>(collection.Where(predicate));
        }
    }
}