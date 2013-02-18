using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using System.Web.WebPages;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Website.Routing
{
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