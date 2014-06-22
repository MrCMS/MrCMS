using System;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
using MrCMS.Entities.Documents.Web;
using NHibernate;

namespace MrCMS.Website.Routing
{
    public class MrCMSAspxHttpHandler : IHttpHandler, IRequiresSessionState
    {
        private readonly ISession _session;
        private readonly IMrCMSRoutingErrorHandler _errorHandler;

        public MrCMSAspxHttpHandler(ISession session, IMrCMSRoutingErrorHandler errorHandler)
        {
            _session = session;
            _errorHandler = errorHandler;
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            // Wrapped up to aid testing
            ProcessRequest(context.Request.RequestContext);
        }

        private void ProcessRequest(RequestContext context)
        {
            string data = Convert.ToString(context.RouteData.Values["data"]);

            UrlHistory urlHistory =
                _session.QueryOver<UrlHistory>()
                    .Where(history => history.UrlSegment == data)
                    .Take(1)
                    .Cacheable()
                    .SingleOrDefault();
            if (urlHistory != null && urlHistory.Webpage != null)
            {
                context.HttpContext.Response.RedirectPermanent("~/" + urlHistory.Webpage.LiveUrlSegment);
            }

            _errorHandler.HandleError(context, 404, new HttpException(404, "Cannot find " + data));
        }
    }
}