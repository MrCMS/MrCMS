using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;

namespace MrCMS.Website.Routing
{
    public sealed class MrCMSHttpHandler : IHttpHandler, IRequiresSessionState
    {
        private readonly IMrCMSRoutingErrorHandler _errorHandler;
        private readonly List<IMrCMSRouteHandler> _routeHandlers;

        public MrCMSHttpHandler(IEnumerable<IMrCMSRouteHandler> routeHandlers, IMrCMSRoutingErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
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
            try
            {
                foreach (var handler in _routeHandlers.OrderByDescending(handler => handler.Priority))
                {
                    if (handler.Handle(context))
                    {
                        return;
                    }
                }
            }
            // for the minimal missing file handler
            catch (ThreadAbortException)
            {
            }
            catch (Exception exception)
            {
                _errorHandler.HandleError(context, 500, new HttpException(500, exception.Message, exception));
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