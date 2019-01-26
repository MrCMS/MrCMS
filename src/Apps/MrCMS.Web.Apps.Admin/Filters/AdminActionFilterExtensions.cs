using System;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Apps.Admin.Filters
{
    public static class AdminActionFilterExtensions
    {
        public static bool IsAdminRequest(this ActionContext context)
        {
            return context.RouteData.Values.ContainsKey("area") &&
                   context.RouteData.Values["area"].ToString().Equals("admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}