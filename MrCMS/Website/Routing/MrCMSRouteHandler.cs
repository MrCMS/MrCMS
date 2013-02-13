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
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var mrCMSHttpHandler = MrCMSApplication.Get<MrCMSHttpHandler>();
	    mrCMSHttpHandler.SetRequestContext(requestContext);
            return mrCMSHttpHandler;
        }
    }
}