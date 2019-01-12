using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public interface IGetBreadcrumbs
    {
        List<PageHeaderBreadcrumb> Get(Type type, int? id);
        List<PageHeaderBreadcrumb> Get(string controllerName, string actionName, int? id);
    }
}