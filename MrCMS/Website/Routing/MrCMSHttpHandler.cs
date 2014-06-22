using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;

namespace MrCMS.Website.Routing
{
    public sealed class MrCMSHttpHandler : IHttpHandler, IRequiresSessionState
    {
        private readonly List<IMrCMSRouteHandler> _routeHandlers;

        public MrCMSHttpHandler(IEnumerable<IMrCMSRouteHandler> routeHandlers)
        {
            _routeHandlers = routeHandlers.ToList();
        }

        public void ProcessRequest(HttpContext context)
        {
            // Wrapped up to aid testing
            ProcessRequest(context.Request.RequestContext);
        }

        public void ProcessRequest(RequestContext context)
        {
            SetCustomHeaders(context);
            foreach (var handler in _routeHandlers.OrderByDescending(handler => handler.Priority))
            {
                if (handler.Handle(context))
                {
                    return;
                }
            }
        }

        private void SetCustomHeaders(RequestContext context)
        {
            context.HttpContext.Response.AppendHeader("X-Built-With", "Mr CMS - http://www.mrcms.com");
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}