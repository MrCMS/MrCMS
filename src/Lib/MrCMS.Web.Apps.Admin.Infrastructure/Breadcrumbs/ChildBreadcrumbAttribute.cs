using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ChildBreadcrumbAttribute : ActionFilterAttribute
    {

        public ChildBreadcrumbAttribute(Type breadcrumbType, string childName = null, string childTitle = null)
        {
            BreadcrumbType = breadcrumbType;
            ChildName = childName;
            ChildTitle = childTitle;
        }

        public Type BreadcrumbType { get; }
        public string ChildName { get; }
        public string ChildTitle { get; }
    }
}