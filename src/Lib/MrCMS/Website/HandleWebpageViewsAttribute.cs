using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public class HandleWebpageViewsAttribute : Attribute
    {
    }

    public class AddWebpageViewsData : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var filterContext = await next();
            var controllerActionDescriptor = (filterContext.ActionDescriptor as ControllerActionDescriptor);
            var controllerAttributes =
                controllerActionDescriptor?.ControllerTypeInfo.GetCustomAttributes<HandleWebpageViewsAttribute>(
                    true) ?? Enumerable.Empty<HandleWebpageViewsAttribute>();
            var methodAttributes = controllerActionDescriptor?.MethodInfo
                                       .GetCustomAttributes<HandleWebpageViewsAttribute>(true) ??
                                   Enumerable.Empty<HandleWebpageViewsAttribute>();
            var customAttributes = methodAttributes.Concat(controllerAttributes);
            if (customAttributes.Any() != true)
            {
                return;
            }

            var result = filterContext.Result as ViewResult;
            if (result == null) return;
            var webpage = result.Model as Webpage;
            if (webpage == null) return;
            await filterContext.HttpContext.RequestServices.GetRequiredService<IProcessWebpageViews>()
                .Process(result, webpage);
        }
    }
}