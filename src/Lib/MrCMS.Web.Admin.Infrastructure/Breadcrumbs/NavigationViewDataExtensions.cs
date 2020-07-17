using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public static class NavigationViewDataExtensions
    {
        public static List<PageHeaderBreadcrumb> Breadcrumbs(this ViewDataDictionary viewData)
        {
            if (!viewData.ContainsKey(BreadcrumbAttribute.Breadcrumbs))
                return new List<PageHeaderBreadcrumb>();

            return viewData[BreadcrumbAttribute.Breadcrumbs] as List<PageHeaderBreadcrumb> ?? new List<PageHeaderBreadcrumb>();
        }
    }
}