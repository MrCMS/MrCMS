using System;
using System.Web;
using System.Web.Mvc.Async;
using System.Web.Routing;
using MrCMS.Settings;

namespace MrCMS.Website.Routing
{
    public class MrCMSRoutingErrorHandler : IMrCMSRoutingErrorHandler
    {
        private readonly IGetErrorPage _getErrorPage;
        private readonly IControllerManager _controllerManager;
        private readonly SiteSettings _siteSettings;

        public MrCMSRoutingErrorHandler(IGetErrorPage getErrorPage, IControllerManager controllerManager, SiteSettings siteSettings)
        {
            _getErrorPage = getErrorPage;
            _controllerManager = controllerManager;
            _siteSettings = siteSettings;
        }

        public void HandleError(RequestContext context, int code, HttpException exception)
        {
            if (ShouldLogException(code))
                HandleExceptionWithElmah(exception);
            var webpage = _getErrorPage.GetPage(code);
            if (webpage != null)
            {
                HttpContextBase httpContext = context.HttpContext;
                httpContext.ClearError();
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = code;
                httpContext.Response.TrySkipIisCustomErrors = true;

                CurrentRequestData.CurrentPage = webpage;
                var controller = _controllerManager.GetController(context, webpage, httpContext.Request.HttpMethod);

                var asyncController = (controller as IAsyncController);
                asyncController.BeginExecute(new RequestContext(httpContext, controller.RouteData), asyncController.EndExecute, null);
            }
            else
            {
                throw exception;
            }
        }

        private bool ShouldLogException(int code)
        {
            return code != 404 || _siteSettings.Log404s;
        }

        private void HandleExceptionWithElmah(Exception exception)
        {
            CurrentRequestData.ErrorSignal.Raise(exception);
        }
    }
}