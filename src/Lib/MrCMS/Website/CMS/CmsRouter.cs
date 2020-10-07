using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Services;
using System;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.CMS
{
    public class CmsRouter : INamedRouter
    {
        private readonly IRouter _defaultRouter;
        public const string IsPreview = "MrCMS-Preview";
        public const string RouteName = "MrCMS Router";

        public CmsRouter(IRouter defaultRouter)
        {
            _defaultRouter = defaultRouter;
        }

        public async Task RouteAsync(RouteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // get the service provider 
            var serviceProvider = context.HttpContext.RequestServices;

            // we only route requests that are of routable methods
            var method = context.HttpContext.Request.Method;
            if (!serviceProvider.GetRequiredService<ICmsMethodTester>().IsRoutable(method))
            {
                return;
            }

            // we want to look at the whole path with the leading '/' stripped
            var url = context.HttpContext.Request.Path.ToUriComponent()?.TrimStart('/');

            var matchResult = serviceProvider.GetRequiredService<ICmsRouteMatcher>().TryMatch(url, method);
            await HandleMatchResult(context, serviceProvider, matchResult);
        }

        private async Task HandleMatchResult(RouteContext context,
            IServiceProvider serviceProvider, CmsMatchData matchResult,
            Action<RouteData> assignAdditionalRouteData = null)
        {
            switch (matchResult.MatchType)
            {
                case CmsRouteMatchType.NoMatch:
                    return;
                case CmsRouteMatchType.Disallowed:
                    await HandleDisallowed(context, serviceProvider);
                    return;
                case CmsRouteMatchType.Preview:
                    context.RouteData.MakePreview();
                    break;
            }

            serviceProvider.GetRequiredService<IAssignPageDataToRouteData>()
                .Assign(context.RouteData, matchResult.PageData);

            assignAdditionalRouteData?.Invoke(context.RouteData);

            context.RouteData.MakeCMSRequest();
            context.RouteData.Routers.Add(_defaultRouter);
            await _defaultRouter.RouteAsync(context);
        }

        public async Task HandlePageExecution(RouteContext routeContext, Webpage webpage,
            Action<RouteData> assignAdditionalRouteData = null)
        {
            var serviceProvider = routeContext.HttpContext.RequestServices;
            var matchResult = serviceProvider.GetRequiredService<ICmsRouteMatcher>()
                .Match(webpage, routeContext.HttpContext.Request.Method);
            if (matchResult.WillRender)
                serviceProvider.GetService<ISetCurrentPage>().SetPage(webpage);
            await HandleMatchResult(routeContext, serviceProvider, matchResult, assignAdditionalRouteData);
        }

        private async Task HandleDisallowed(RouteContext context, IServiceProvider serviceProvider)
        {
            var getErrorPage = serviceProvider.GetRequiredService<IGetErrorPage>();
            var currentUser = serviceProvider.GetRequiredService<IGetCurrentUser>().Get();

            var code = currentUser != null ? 403 : 401;
            context.HttpContext.Response.StatusCode = code;
            var webpage = getErrorPage.GetPage(code);
            if (webpage == null)
            {
                await context.HttpContext.Response.WriteAsync(code.ToString());
                return;
            }

            var documentMetadataService = serviceProvider.GetRequiredService<IDocumentMetadataService>();

            var metadata = documentMetadataService.GetMetadata(webpage);
            context.RouteData.Values["controller"] = metadata.WebGetController;
            context.RouteData.Values["action"] = metadata.WebGetAction;
            context.RouteData.Values["id"] = webpage.Id;
            await _defaultRouter.RouteAsync(context);
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            var url = context.Values["data"]?.ToString();
            if (!string.IsNullOrWhiteSpace(url))
            {
                var values = new RouteValueDictionary(context.Values);
                values.Remove("data");
                url = context.HttpContext.RequestServices.GetRequiredService<IQuerySerializer>()
                    .AppendToUrl(url, values);
                return new VirtualPathData(this, url);
            }

            return _defaultRouter.GetVirtualPath(context);
        }

        public string Name => RouteName;
    }
}