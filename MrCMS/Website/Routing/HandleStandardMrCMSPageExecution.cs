using System;
using System.Web.Mvc.Async;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;
using MrCMS.Website.Controllers;
using MrCMS.Website.Optimization;

namespace MrCMS.Website.Routing
{
    public class HandleStandardMrCMSPageExecution : IHandleStandardMrCMSPageExecution
    {
        private readonly IControllerManager _controllerManager;
        private readonly SEOSettings _seoSettings;

        public HandleStandardMrCMSPageExecution(IControllerManager controllerManager, SEOSettings seoSettings)
        {
            _controllerManager = controllerManager;
            _seoSettings = seoSettings;
        }

        public void Handle(RequestContext context, Webpage webpage, Action<MrCMSUIController> beforeExecute)
        {
            var controller = _controllerManager.GetController(context, webpage, context.HttpContext.Request.HttpMethod);

            _controllerManager.SetFormData(webpage, controller, context.HttpContext.Request.Form);

            if (beforeExecute != null)
            {
                var uiController = controller as MrCMSUIController;
                if (uiController != null)
                    beforeExecute(uiController);
            }

            if (_seoSettings.EnableHtmlMinification)
                context.HttpContext.Response.Filter = new WhitespaceFilter(context.HttpContext.Response.Filter);
            var asyncController = (controller as IAsyncController);
            asyncController.BeginExecute(context, asyncController.EndExecute, null);
        }
    }
}