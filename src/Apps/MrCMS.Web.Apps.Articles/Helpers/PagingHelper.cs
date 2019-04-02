using System.Collections.Generic;
using X.PagedList;

// todo: move core?
namespace MrCMS.Web.Apps.Articles.Helpers
{
    public static class PagingHelper
    {
        public static IPagedList<T> EmptyList<T>()
        {
            return new PagedList<T>(new List<T>(), 1, 1);
        }
    }
}
