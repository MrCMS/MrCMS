using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services;

namespace MrCMS.Website.Filters
{
    public class ReturnUrlFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var filterContext = await next();

            var controllerActionDescriptor = (filterContext.ActionDescriptor as ControllerActionDescriptor);
            var controllerAttributes =
                controllerActionDescriptor?.ControllerTypeInfo.GetCustomAttributes<ReturnUrlAttribute>(
                    true) ?? Enumerable.Empty<ReturnUrlAttribute>();
            var methodAttributes = controllerActionDescriptor?.MethodInfo
                .GetCustomAttributes<ReturnUrlAttribute>(true) ?? Enumerable.Empty<ReturnUrlAttribute>();
            var customAttributes = methodAttributes.Concat(controllerAttributes);
            if (customAttributes.Any() != true)
            {
                return;
            }

            if (filterContext.Result is not RedirectToRouteResult && filterContext.Result is not RedirectResult &&
                filterContext.Result is not RedirectToActionResult)
                return;

            string returnUrl = null;
            if (filterContext.HttpContext.Request.HasFormContentType &&
                filterContext.HttpContext.Request.Form[ReturnUrlAttribute.ReturnUrlKey].FirstOrDefault() != null)
            {
                returnUrl = filterContext.HttpContext.Request.Form[ReturnUrlAttribute.ReturnUrlKey].FirstOrDefault();
            }

            if (returnUrl == null)
            {
                returnUrl = filterContext.HttpContext.Request.Query[ReturnUrlAttribute.ReturnUrlKey].FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(returnUrl))
                return;

            var resultUrl = returnUrl;
            // convert absolute urls within the current site to local, so we can use local redirect result to filter
            // off site redirects
            if (Uri.TryCreate(returnUrl, UriKind.RelativeOrAbsolute, out var uri))
            {
                if (uri.IsAbsoluteUri)
                {
                    var currentSiteLocator = context.HttpContext.RequestServices.GetRequiredService<ICurrentSiteLocator>();
                    var site = currentSiteLocator.GetCurrentSite();
                    var authority = uri.Authority;
                    if (site.BaseUrl != null && (site.BaseUrl.Equals(authority, StringComparison.OrdinalIgnoreCase) ||
                                                 site.StagingUrl != null &&
                                                 site.StagingUrl.Equals(authority, StringComparison.OrdinalIgnoreCase)))
                    {
                        resultUrl = uri.PathAndQuery;
                    }
                }
            }

            filterContext.Result = new LocalRedirectResult(resultUrl);
        }
    }
}