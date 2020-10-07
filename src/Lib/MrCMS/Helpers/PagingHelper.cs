using System.Collections.Generic;
using X.PagedList;

namespace MrCMS.Helpers
{
    public static class PagingHelper
    {
        public static IPagedList<T> EmptyList<T>()
        {
            return new PagedList<T>(new List<T>(), 1, 1);
        }
    }
}
