using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Website.Routing
{
    public class MrCMSAspxRouteHandler : MvcRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (CurrentRequestData.DatabaseIsInstalled)
            {
                var mrCMSHttpHandler = MrCMSApplication.Get<MrCMSAspxHttpHandler>();
                mrCMSHttpHandler.SetRequestContext(requestContext);
                return mrCMSHttpHandler;
            }
            else
            {
                return new NotInstalledHandler();
            }
        }
    }

    public class MrCMSAspxHttpHandler : ErrorHandlingHttpHandler
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;

        public MrCMSAspxHttpHandler(ISession session, IDocumentService documentService, IControllerManager controllerManager, SiteSettings siteSettings)
            : base(documentService, controllerManager)
        {
            _session = session;
            _siteSettings = siteSettings;
        }

        public override void ProcessRequest(HttpContext context)
        {
            // Wrapped up to aid testing
            ProcessRequest(new HttpContextWrapper(context));
        }

        public override bool IsReusable
        {
            get { return false; }
        }

        private void ProcessRequest(HttpContextWrapper context)
        {
            var urlHistory =
                _session.QueryOver<UrlHistory>()
                        .Where(history => history.UrlSegment == Data && history.Site.Id == _siteSettings.Site.Id)
                        .Take(1)
                        .Cacheable()
                        .SingleOrDefault();
            if (urlHistory != null && urlHistory.Webpage != null)
            {
                context.Response.RedirectPermanent("~/" + urlHistory.Webpage.LiveUrlSegment);
            }

            HandleError(context, 404, _siteSettings.Error404PageId, new HttpException(404, "Cannot find " + Data), _siteSettings.Log404s);
        }

        private string Data
        {
            get { return Convert.ToString(RequestContext.RouteData.Values["data"]); }
        }
    }

    public class MrCMSRouteHandler : MvcRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (CurrentRequestData.DatabaseIsInstalled)
            {
                var mrCMSHttpHandler = MrCMSApplication.Get<MrCMSHttpHandler>();
                mrCMSHttpHandler.SetRequestContext(requestContext);
                return mrCMSHttpHandler;
            }
            else
            {
                return new NotInstalledHandler();
            }
        }
    }

    public class NotInstalledHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Redirect("~/Install");
        }

        public bool IsReusable { get; private set; }
    }
}