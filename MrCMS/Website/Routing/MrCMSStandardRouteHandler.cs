using System;
using System.Web;
using System.Web.Mvc.Async;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;
using MrCMS.Website.Optimization;
using NHibernate;

namespace MrCMS.Website.Routing
{
    public class MrCMSStandardRouteHandler : IMrCMSRouteHandler
    {
        private readonly IGetWebpageForRequest _getWebpageForRequest;
        private readonly IControllerManager _controllerManager;
        private readonly ISession _session;
        private readonly SEOSettings _seoSettings;
        private readonly IMrCMSRoutingErrorHandler _errorHandler;

        public MrCMSStandardRouteHandler(IGetWebpageForRequest getWebpageForRequest,
            IControllerManager controllerManager, ISession session, SEOSettings seoSettings,
            IMrCMSRoutingErrorHandler errorHandler)
        {
            _getWebpageForRequest = getWebpageForRequest;
            _controllerManager = controllerManager;
            _session = session;
            _seoSettings = seoSettings;
            _errorHandler = errorHandler;
        }

        public int Priority { get { return 1000; } }
        public bool Handle(RequestContext context)
        {
            Webpage webpage = _getWebpageForRequest.Get(context);
            if (webpage == null)
            {
                return false;
            }
            var controller = _controllerManager.GetController(context, webpage, context.HttpContext.Request.HttpMethod);

            _controllerManager.SetFormData(webpage, controller, context.HttpContext.Request.Form);

            try
            {
                if (_seoSettings.EnableHtmlMinification)
                    context.HttpContext.Response.Filter = new WhitespaceFilter(context.HttpContext.Response.Filter);
                var asyncController = (controller as IAsyncController);
                asyncController.BeginExecute(context, asyncController.EndExecute, null);
                return true;
            }
            catch (Exception exception)
            {
                _errorHandler.HandleError(context, 500, new HttpException(500, exception.Message, exception));
            }
            return false;
        }
    }
}