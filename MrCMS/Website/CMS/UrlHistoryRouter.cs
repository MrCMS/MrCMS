using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Website.CMS
{
    public class UrlHistoryRouter : INamedRouter
    {
        private readonly IRouter _defaultRouter;
        //private readonly Func<ICmsUrlHistoryMatcher> _urlHistoryMatcher;
        //private readonly ICmsMethodTester _methodTester;

        public UrlHistoryRouter(IRouter defaultRouter)//, Func<ICmsUrlHistoryMatcher> urlHistoryMatcher, ICmsMethodTester methodTester)
        {
            _defaultRouter = defaultRouter;
            //_urlHistoryMatcher = urlHistoryMatcher;
            //_methodTester = methodTester;
        }

        public async Task RouteAsync(RouteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            // get the service provider 
            var serviceProvider = context.HttpContext.RequestServices;


            // we only route requests that are of routable methods
            if (!serviceProvider.GetRequiredService<ICmsMethodTester>().IsRoutable(context.HttpContext.Request.Method))
                return;

            // we want to look at the whole path with the leading '/' stripped
            var url = context.HttpContext.Request.Path.ToUriComponent()?.TrimStart('/');

            // we will leave the homepage to be explicitly routed 
            if (string.IsNullOrWhiteSpace(url))
                return;

            var result = await serviceProvider.GetRequiredService<ICmsUrlHistoryMatcher>().TryMatch(url);
            if (!result.Match)
                return;


            // if we've matched, we'll 301 to the correct url
            Task ContextHandler(HttpContext httpContext)
            {
                httpContext.Response.Redirect(result.RedirectUrl, true);
                return Task.CompletedTask;
            }

            context.Handler = ContextHandler;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return _defaultRouter.GetVirtualPath(context);
        }

        public string Name => "MrCMS Url History Router";
    }
}