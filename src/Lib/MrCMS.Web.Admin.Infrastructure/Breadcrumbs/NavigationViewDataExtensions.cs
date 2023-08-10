using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public static class NavigationViewDataExtensions
    {
        public static List<PageHeaderBreadcrumb> Breadcrumbs(this ViewDataDictionary viewData)
        {
            if (!viewData.ContainsKey(BreadcrumbKey))
                return new List<PageHeaderBreadcrumb>();

            return viewData[BreadcrumbKey] as List<PageHeaderBreadcrumb> ?? new List<PageHeaderBreadcrumb>();
        }

        public const string BreadcrumbKey = "breadcrumbs";
    }
}