using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public interface IGetBreadcrumbTypes
    {
        List<Type> GetHierarchy(Type type);
        Type FindBreadcrumbType(string controllerName, string actionName);
    }
}