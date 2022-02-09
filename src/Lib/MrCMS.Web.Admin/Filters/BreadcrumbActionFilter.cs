using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;
using MrCMS.Website;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Web.Admin.Filters
{
    public class BreadcrumbActionFilter : IAsyncActionFilter
    {
        // private readonly IGetBreadcrumbs _getBreadcrumbs;
        // private readonly IGetControllerFromFilterContext _getControllerFromFilterContext;
        //
        // public BreadcrumbActionFilter(IGetBreadcrumbs getBreadcrumbs,
        //     IGetControllerFromFilterContext getControllerFromFilterContext
        //     )
        // {
        //     _getBreadcrumbs = getBreadcrumbs;
        //     _getControllerFromFilterContext = getControllerFromFilterContext;
        // }

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

            var serviceProvider = context.HttpContext.RequestServices;
            var controller = serviceProvider.GetRequiredService<IGetControllerFromFilterContext>()
                .GetController(executedContext);
            var pageHeaderBreadcrumbs = serviceProvider.GetRequiredService<IGetBreadcrumbs>().Get(
                controllerActionDescriptor.ControllerName, controllerActionDescriptor.ActionName,
                context.ActionArguments);
            controller.ViewData[BreadcrumbAttribute.Breadcrumbs] =
                pageHeaderBreadcrumbs;
        }
    }
}