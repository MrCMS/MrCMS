using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Website.Routing
{
    public class MrCMSRouteHandler : MvcRouteHandler
    {
        private readonly Func<ISession> _session;
        private readonly Func<IDocumentService> _documentService;
        private readonly Func<SiteSettings> _siteSettings;

        public MrCMSRouteHandler(Func<ISession> session, Func<IDocumentService> documentService, Func<SiteSettings> siteSettings)
        {
            _session = session;
            _documentService = documentService;
            _siteSettings = siteSettings;
        }

        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MrCMSHttpHandler(requestContext, _session, _documentService, _siteSettings);
        }
    }
}