using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Helpers
{
    public static class EnumerableHelper
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action.Invoke(item);
            }
        }

        public static List<T> ToList<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            return list.Where(predicate).ToList();
        }
    }
}