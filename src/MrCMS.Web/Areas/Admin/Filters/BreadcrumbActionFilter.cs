﻿using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Filters
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

            var controller = _getControllerFromFilterContext.GetController(executedContext);
            controller.ViewData[BreadcrumbAttribute.Breadcrumbs] = await _getBreadcrumbs.Get(controllerActionDescriptor.ControllerName, controllerActionDescriptor.ActionName,
                    context.ActionArguments);
        }
    }
}