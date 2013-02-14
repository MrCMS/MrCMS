using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.Routing
{
    public class ErrorHandler
    {
        private readonly IControllerManager _controllerManager;

        public ErrorHandler(IControllerManager controllerManager)
        {
            _controllerManager = controllerManager;
        }

        private void HandleExceptionWithElmah(Exception exception)
        {
            CurrentRequestData.ErrorSignal.Raise(exception);
        }

        public void HandleError(Webpage webpage, RequestContext requestContext, HttpContextBase context, int code,  HttpException exception)
        {
            HandleExceptionWithElmah(exception);
            if (webpage != null)
            {
                context.ClearError();
                context.Response.Clear();
                context.Response.StatusCode = code;
                context.Response.TrySkipIisCustomErrors = true;

                var data = new RouteData();
                data.Values["data"] = webpage.LiveUrlSegment;

                var controller = _controllerManager.GetController(requestContext, webpage, context.Request.HttpMethod);
                (controller as IController).Execute(new RequestContext(context, controller.RouteData));
            }
            else
            {
                throw exception;
            }
        }
    }
}