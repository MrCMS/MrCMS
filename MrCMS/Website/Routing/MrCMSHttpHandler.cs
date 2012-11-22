using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Elmah;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website.Controllers;

namespace MrCMS.Website.Routing
{
    internal sealed class MrCMSHttpHandler : IHttpHandler, IRequiresSessionState
    {
        private readonly RequestContext _requestContext;
        private readonly Func<IDocumentService> _documentService;
        private readonly Func<SiteSettings> _siteSettings;

        public MrCMSHttpHandler(RequestContext requestContext, Func<IDocumentService> documentService, Func<SiteSettings> siteSettings)
        {
            _requestContext = requestContext;
            _documentService = documentService;
            _siteSettings = siteSettings;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (!MrCMSApplication.DatabaseIsInstalled())
                context.Response.Redirect("~/Install");

            var documentService = _documentService.Invoke();
            var siteSettings = _siteSettings.Invoke();

            if (MrCMSApplication.UserLoggedIn
                    ? !documentService.AnyWebpages()
                    : !documentService.AnyPublishedWebpages())
                context.Response.Redirect("~/Admin");


            var extension = Path.GetExtension(_requestContext.HttpContext.Request.Url.ToString());
            if (!string.IsNullOrWhiteSpace(extension))
                return;

            var webpage = GetWebpage(_requestContext);
            if (webpage is Redirect)
            {
                context.Response.Redirect("~/" + (webpage as Redirect).RedirectUrl);
                return;
            }

            if (webpage == null)
            {
                var error404 = documentService.GetDocument<Webpage>(siteSettings.Error404PageId);
                if (error404 != null)
                {
                    context.Response.Redirect("~/" + error404.LiveUrlSegment);
                }
                else
                {
                    context.Response.Redirect("~");
                }
                return;
            }
            if (!webpage.IsAllowed(MrCMSApplication.CurrentUser))
            {
                context.Response.Redirect("~");
                return;
            }

            Controller controller = GetController(_requestContext);
            controller.RouteData.Values["controller"] = GetControllerName(_requestContext);
            controller.RouteData.Values["action"] = GetActionName(_requestContext);
            controller.RouteData.Values["page"] = webpage;

            if (_requestContext.HttpContext.Request.HttpMethod == "POST")
            {
                if (_requestContext.HttpContext.Request.Form != null)
                    controller.RouteData.Values["form"] =
                        new FormCollection(_requestContext.HttpContext.Request.Form);
            }
            try
            {
                (controller as IController).Execute(_requestContext);
            }
            catch (Exception exception)
            {
                var errorSignal = ErrorSignal.FromContext(context);
                errorSignal.Raise(exception);

                if (!HttpContext.Current.IsDebuggingEnabled)
                    context.Response.Redirect("~/500");
            }
        }

        private string GetActionName(RequestContext requestContext)
        {
            var webpage = GetWebpage(requestContext);

            if (webpage == null)
                return null;

            if (!webpage.Published && !MrCMSApplication.UserLoggedIn)
                return null;

            var definition = DocumentTypeHelper.GetDefinitionByType(webpage.GetType());

            if (definition == null) return null;

            string actionName;

            switch (requestContext.HttpContext.Request.HttpMethod)
            {
                case "GET":
                    actionName = definition.WebGetAction;
                    break;
                case "POST":
                    actionName = definition.WebPostAction;
                    break;
                default:
                    return null;
            }

            return actionName;
        }

        private Controller GetController(RequestContext requestContext)
        {
            var controllerName = GetControllerName(requestContext);

            var controller = ControllerBuilder.Current.GetControllerFactory().CreateController(_requestContext, controllerName ?? "MrCMS") as Controller;
            controller.ControllerContext = new ControllerContext(_requestContext, controller) { RouteData = _requestContext.RouteData };
            return controller;
        }

        private string GetControllerName(RequestContext requestContext)
        {
            var webpage = GetWebpage(requestContext);

            if (webpage == null)
                return null;

            if (!webpage.Published && !MrCMSApplication.UserLoggedIn)
                return null;

            var definition = DocumentTypeHelper.GetDefinitionByType(webpage.GetType());

            if (definition == null) return null;

            string controllerName;

            switch (requestContext.HttpContext.Request.HttpMethod)
            {
                case "GET":
                    controllerName = definition.WebGetController;
                    break;
                case "POST":
                    controllerName = definition.WebPostController;
                    break;
                default:
                    return null;
            }

            return controllerName;
        }

        private Webpage GetWebpage(RequestContext requestContext)
        {
            var documentService = _documentService.Invoke();
            var data = Convert.ToString(requestContext.RouteData.Values["data"]);

            var webpage = string.IsNullOrWhiteSpace(data)
                              ? !MrCMSApplication.UserLoggedIn
                                    ? MrCMSApplication.PublishedRootChildren.FirstOrDefault()
                                    : MrCMSApplication.RootChildren.FirstOrDefault()
                              : documentService.GetDocumentByUrl<Webpage>(data);

            MrCMSApplication.CurrentPage = webpage;
            return webpage;
        }

        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }
    }
}