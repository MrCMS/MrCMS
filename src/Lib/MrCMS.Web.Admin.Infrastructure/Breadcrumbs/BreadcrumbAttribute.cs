using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BreadcrumbAttribute : ActionFilterAttribute
    {

        public const string Breadcrumbs = "breadcrumbs";
        public BreadcrumbAttribute(Type breadcrumbType)
        {
            BreadcrumbType = breadcrumbType;
        }

        public Type BreadcrumbType { get; }
    }
}