using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Website
{
    public static class HtmlHelperExtensions
    {
        public static T Get<T>(this HtmlHelper helper)
        {
            return helper.ViewContext.HttpContext.Get<T>();
        }

        public static IEnumerable<T> GetAll<T>(this HtmlHelper helper)
        {
            return helper.ViewContext.HttpContext.GetAll<T>();
        }
    }
}