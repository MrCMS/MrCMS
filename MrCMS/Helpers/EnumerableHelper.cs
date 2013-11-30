using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Website;

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

        /// <summary>
        /// Break a list of items into chunks of a specific size
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize).ToList();
            }
        }

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int page, int? pageSize = null)
        {
            return new PagedList<T>(source, page, pageSize ?? MrCMSApplication.Get<SiteSettings>().DefaultPageSize);
        }
    }
}