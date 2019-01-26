using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Entities.Multisite;

namespace MrCMS.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string AbsoluteAction(this IUrlHelper helper, string action, string controller, object values)
        {
            return helper.Action(action, controller, values, helper.ActionContext.HttpContext.Request.Scheme);
        }

        public static string AbsoluteAction(this IUrlHelper helper, string action, string controller, object values, Site site)
        {
            return helper.Action(action, controller, values, helper.ActionContext.HttpContext.Request.Scheme,
                site.BaseUrl.Trim('/'));
        }
    }
}