using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Website.CMS
{
    public class CmsRouter : INamedRouter
    {
        private readonly IRouter _defaultRouter;
        //private readonly Func<ICmsRouteMatcher> _routeMatcher;
        //private readonly ICmsMethodTester _methodTester;
        //private readonly IAssignPageDataToRouteData _assignPageDataToRouteData;
        //private readonly IQuerySerializer _querySerializer;
        public const string IsPreview = "MrCMS-Preview";
        public const string RouteName = "MrCMS Router";

        public CmsRouter(IRouter defaultRouter)//, ICmsMethodTester methodTester, IAssignPageDataToRouteData assignPageDataToRouteData, IQuerySerializer querySerializer)
        {
            _defaultRouter = defaultRouter;
            //_methodTester = methodTester;
            //_assignPageDataToRouteData = assignPageDataToRouteData;
            //_querySerializer = querySerializer;
        }

        public async Task RouteAsync(RouteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            // get the service provider 
            var serviceProvider = context.HttpContext.RequestServices;

            // we only route requests that are of routable methods
            var method = context.HttpContext.Request.Method;
            if (!serviceProvider.GetRequiredService<ICmsMethodTester>().IsRoutable(method))
                return;

            // we want to look at the whole path with the leading '/' stripped
            var url = context.HttpContext.Request.Path.ToUriComponent()?.TrimStart('/');

            //// we will leave the homepage to be explicitly routed 
            //if (string.IsNullOrWhiteSpace(url))
            //    return;
            var matchResult = await serviceProvider.GetRequiredService<ICmsRouteMatcher>().TryMatch(url, method);
            if (matchResult.MatchType == CmsRouteMatchType.NoMatch)
                return;
            if (matchResult.MatchType == CmsRouteMatchType.Preview)
                context.RouteData.MakePreview();
            serviceProvider.GetRequiredService<IAssignPageDataToRouteData>().Assign(context.RouteData, matchResult.PageData);

            context.RouteData.MakeCMSRequest();
            context.RouteData.Routers.Add(_defaultRouter);
            await _defaultRouter.RouteAsync(context);
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            var url = context.Values["data"]?.ToString();
            if (!string.IsNullOrWhiteSpace(url))
            {
                var values = new RouteValueDictionary(context.Values);
                values.Remove("data");
                url = context.HttpContext.RequestServices.GetRequiredService<IQuerySerializer>().AppendToUrl(url, values);
                return new VirtualPathData(this, url);
            }
            return _defaultRouter.GetVirtualPath(context);

            // TODO: Decide whether we want to validate this
            //var matchResult = _routeMatcher().TryMatch(url).GetAwaiter().GetResult();
            //if (matchResult.MatchType == CmsRouteMatchType.NoMatch)
            //    return _defaultRouter.GetVirtualPath(context);
        }

        public string Name => RouteName;
    }
}