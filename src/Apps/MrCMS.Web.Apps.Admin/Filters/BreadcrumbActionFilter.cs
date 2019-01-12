using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Entities;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;
using MrCMS.Website;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Filters
{
    public class BreadcrumbActionFilter : IAsyncActionFilter
    {
        private readonly IGetBreadcrumbs _getBreadcrumbs;
        private readonly IGetControllerFromFilterContext _getControllerFromFilterContext;

        public BreadcrumbActionFilter(IGetBreadcrumbs getBreadcrumbs,
            IGetControllerFromFilterContext getControllerFromFilterContext
            )
        {
            _getBreadcrumbs = getBreadcrumbs;
            _getControllerFromFilterContext = getControllerFromFilterContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();
            // not interested in these if it's not a GET
            if (context.HttpContext.Request.Method != HttpMethod.Get.Method)
            {
                return;
            }

            var controllerActionDescriptor = (executedContext.ActionDescriptor as ControllerActionDescriptor);
            if (controllerActionDescriptor == null)
            {
                return;
            }

            var id = GetId(context);

            var controller = _getControllerFromFilterContext.GetController(executedContext);
            controller.ViewData[BreadcrumbAttribute.Breadcrumbs] =
                _getBreadcrumbs.Get(controllerActionDescriptor.ControllerName, controllerActionDescriptor.ActionName,
                    id);
        }

        private static int? GetId(ActionExecutingContext context)
        {
            int? id = null;
            if (context.ActionArguments.ContainsKey("id") &&
                int.TryParse(context.ActionArguments["id"]?.ToString(), out int idVal))
            {
                id = idVal;
            }
            else if (context.ActionArguments.Values.OfType<SystemEntity>().Any())
            {
                id = context.ActionArguments.Values.OfType<SystemEntity>().First().Id;
            }
            else if (context.ActionArguments.Values.OfType<IHaveId>().Any())
            {
                id = context.ActionArguments.Values.OfType<IHaveId>().First().Id;
            }

            return id;
        }
    }
}