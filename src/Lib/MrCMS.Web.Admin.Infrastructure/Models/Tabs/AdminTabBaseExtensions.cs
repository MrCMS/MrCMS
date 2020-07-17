using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities;

namespace MrCMS.Web.Admin.Infrastructure.Models.Tabs
{
    public static class AdminTabBaseExtensions
    {
        public static string Name<T>(this AdminTabBase<T> tab, IHtmlHelper helper, T entity) where T : SystemEntity
        {
            return tab.Name(helper.ViewContext.HttpContext.RequestServices, entity);
        }

        public static bool ShouldShow<T>(this AdminTabBase<T> tab, IHtmlHelper helper, T entity) where T : SystemEntity
        {
            return tab.ShouldShow(helper.ViewContext.HttpContext.RequestServices, entity);
        }

    }
}