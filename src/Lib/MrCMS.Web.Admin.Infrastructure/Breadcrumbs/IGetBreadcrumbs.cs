using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public interface IGetBreadcrumbs
    {
        Task<List<PageHeaderBreadcrumb>> Get(Type type, IDictionary<string, object> actionArguments);
        Task<List<PageHeaderBreadcrumb>> Get(string controllerName, string actionName,
            IDictionary<string, object> actionArguments);
    }
}