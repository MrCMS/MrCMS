using System;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public class PageHeaderBreadcrumb
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public Func<IUrlHelper, string> Link { get; set; }
        //public bool HasNoLink => string.IsNullOrWhiteSpace(Link);
        public Type BreadcrumbType { get; set; }
    }
}