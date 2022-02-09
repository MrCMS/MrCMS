using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities;

namespace MrCMS.Web.Admin.Infrastructure.Models.Tabs
{
    public static class AdminTabBaseExtensions
    {
        public static async Task<string> Name<T>(this AdminTabBase<T> tab, IHtmlHelper helper, T entity) where T : SystemEntity
        {
            return await tab.Name(helper.ViewContext.HttpContext.RequestServices, entity);
        }

        public static async Task<bool> ShouldShow<T>(this AdminTabBase<T> tab, IHtmlHelper helper, T entity) where T : SystemEntity
        {
            return await tab.ShouldShow(helper.ViewContext.HttpContext.RequestServices, entity);
        }

    }
}