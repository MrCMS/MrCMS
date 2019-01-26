using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public interface IGetBreadcrumbs
    {
        List<PageHeaderBreadcrumb> Get(Type type, IDictionary<string, object> actionArguments);
        List<PageHeaderBreadcrumb> Get(string controllerName, string actionName, IDictionary<string, object> actionArguments);
    }
}