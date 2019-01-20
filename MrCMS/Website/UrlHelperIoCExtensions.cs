using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Website
{
    public static class UrlHelperIoCExtensions
    {
        public static T Get<T>(this UrlHelper helper)
        {
            return helper.RequestContext.HttpContext.Get<T>();
        }

        public static IEnumerable<T> GetAll<T>(this UrlHelper helper)
        {
            return helper.RequestContext.HttpContext.GetAll<T>();
        }
    }
}