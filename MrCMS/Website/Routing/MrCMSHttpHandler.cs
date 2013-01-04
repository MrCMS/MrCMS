using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Elmah;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Website.Routing
{
    public sealed class MrCMSHttpHandler : IHttpHandler, IRequiresSessionState
    {
        private readonly RequestContext _requestContext;
        private readonly Func<ISession> _getSession;
        private readonly Func<IDocumentService> _getDocumentService;
        private readonly Func<SiteSettings> _getSiteSettings;
        private IDocumentService _documentService;
        private SiteSettings _siteSettings;
        private string _httpMethod;
        private Webpage _webpage;
        private bool _webpageLookedUp;
        private ISession _session;

        public MrCMSHttpHandler(RequestContext requestContext, Func<ISession> getSession, Func<IDocumentService> getDocumentService, Func<SiteSettings> getSiteSettings)
        {
            _requestContext = requestContext;
            _getSession = getSession;
            _getDocumentService = getDocumentService;
            _getSiteSettings = getSiteSettings;
        }

        public string HttpMethod
        {
            get { return _httpMethod ?? RequestContext.HttpContext.Request.HttpMethod; }
            set { _httpMethod = value; }
        }

        public void ProcessRequest(HttpContext context)
        {
            // Wrapped up to aid testing
            ProcessRequest(new HttpContextWrapper(context));
        }

        public void ProcessRequest(HttpContextBase context)
        {
            if (!CheckIsInstalled(context)) return;

            if (CheckIsFile()) return;

            if (Handle404(context)) return;

            if (PageIsRedirect(context)) return;

            if (RedirectsToHomePage(context)) return;

            if (!IsAllowed(context)) return;

            var controller = GetController();

            SetViewModel(controller);

            SetFormData(controller);

            try
            {
                (controller as IController).Execute(RequestContext);
            }
            catch (Exception exception)
            {
                Handle500(context, exception);
            }
        }

        public void SetViewModel(Controller controller)
        {
            if (HttpMethod == "GET" && Webpage != null)
            {
                Webpage.UiViewData(controller.ViewData, Session, controller.Request);
            }
        }

        public bool RedirectsToHomePage(HttpContextBase context)
        {
            if (Webpage.LiveUrlSegment != Webpage.UrlSegment && context.Request.Url.AbsolutePath != "/")
            {
                context.Response.Redirect("~/" + Webpage.LiveUrlSegment);
                return true;
            }
            return false;
        }

        public void SetFormData(Controller controller)
        {
            if (HttpMethod == "POST" && RequestContext.HttpContext.Request.Form != null)
            {
                var formCollection = new FormCollection(RequestContext.HttpContext.Request.Form);
                if (WebpageDefinition != null && WebpageDefinition.PostTypes != null && WebpageDefinition.PostTypes.Any())
                {
                    foreach (var type in WebpageDefinition.PostTypes)
                    {
                        var modelBinder = ModelBinders.Binders.GetBinder(type) as MrCMSDefaultModelBinder;
                        if (modelBinder != null)
                        {
                            var modelBindingContext = new ModelBindingContext
                                                          {
                                                              ValueProvider =
                                                                  formCollection,
                                                              ModelMetadata =
                                                                  ModelMetadataProviders.Current.GetMetadataForType(
                                                                      () =>
                                                                      modelBinder.GetModelFromSession(
                                                                          controller.ControllerContext,
                                                                          string.Empty, type), type)
                                                          };

                            var model = modelBinder.BindModel(controller.ControllerContext, modelBindingContext);
                            controller.RouteData.Values[type.Name.ToLower()] = model;
                        }
                    }
                }
                else
                {
                    controller.RouteData.Values["form"] = formCollection;
                }
            }
        }

        public void Handle500(HttpContextBase context, Exception exception)
        {
            HandleExceptionWithElmah(context, exception);

            var error500 = DocumentService.GetDocument<Webpage>(SiteSettings.Error500PageId);
            if (error500 != null)
            {
                context.Response.Redirect("~/" + error500.LiveUrlSegment);
            }
            else
            {
                context.Response.Redirect("~");
            }
        }

        private void HandleExceptionWithElmah(HttpContextBase context, Exception exception)
        {
            var errorSignal = ErrorSignal.Get(context.ApplicationInstance);
            errorSignal.Raise(exception);
        }

        public bool IsAllowed(HttpContextBase context)
        {
            if (!Webpage.IsAllowed(MrCMSApplication.CurrentUser))
            {
                context.Response.Redirect("~");
                return false;
            }
            return true;
        }

        public bool Handle404(HttpContextBase context)
        {
            if (Webpage == null)
            {
                var error404 = DocumentService.GetDocument<Webpage>(SiteSettings.Error404PageId);
                if (error404 != null)
                {
                    context.Response.Redirect("~/" + error404.LiveUrlSegment);
                }
                else
                {
                    context.Response.Redirect("~");
                }
                return true;
            }
            return false;
        }

        public bool PageIsRedirect(HttpContextBase context)
        {
            if (Webpage is Redirect)
            {
                string redirectUrl = (Webpage as Redirect).RedirectUrl;
                Uri result;
                if (Uri.TryCreate(redirectUrl, UriKind.Absolute, out result))
                    context.Response.Redirect(redirectUrl);
                else
                {
                    if (redirectUrl.StartsWith("/"))
                        redirectUrl = redirectUrl.Substring(1);

                    context.Response.Redirect("~/" + redirectUrl);
                }
                return true;
            }
            return false;
        }

        public bool CheckIsFile()
        {
            var path = RequestContext.HttpContext.Request.Url.ToString();
            var extension = Path.GetExtension(path);
            if (!string.IsNullOrWhiteSpace(extension))
                return true;
            return false;
        }

        public bool CheckIsInstalled(HttpContextBase context)
        {
            if (!MrCMSApplication.DatabaseIsInstalled)
            {
                context.Response.Redirect("~/Install");
                return false;
            }
            return true;
        }

        public string GetActionName()
        {
            if (Webpage == null)
                return null;

            if (!Webpage.Published && !Webpage.IsAllowedForAdmin(MrCMSApplication.CurrentUser))
                return null;

            var definition = DocumentService.GetDefinitionByType(Webpage.GetType());

            if (definition == null) return null;

            switch (HttpMethod)
            {
                case "GET":
                    return definition.WebGetAction;
                case "POST":
                    return definition.WebPostAction;
                default:
                    return null;
            }
        }

        public Controller GetController()
        {
            var controllerName = GetControllerName();

            var controller = ControllerBuilder.Current.GetControllerFactory().CreateController(RequestContext, controllerName ?? "MrCMS") as Controller;
            controller.ControllerContext = new ControllerContext(RequestContext, controller) { RouteData = RequestContext.RouteData };

            var routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary["controller"] = controllerName;
            routeValueDictionary["action"] = GetActionName();
            routeValueDictionary["page"] = Webpage;
            controller.RouteData.Values.Merge(routeValueDictionary);

            return controller;
        }

        public string GetControllerName()
        {
            if (Webpage == null)
                return null;

            if (!Webpage.Published && !Webpage.IsAllowedForAdmin(MrCMSApplication.CurrentUser))
                return null;

            var definition = DocumentService.GetDefinitionByType(Webpage.GetType());

            if (definition == null) return null;

            string controllerName;

            switch (HttpMethod)
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

        public Webpage Webpage
        {
            get { return _webpage ?? (!_webpageLookedUp ? (_webpage = GetWebpage()) : null); }
            set
            {
                _webpage = value;
                _webpageLookedUp = true;
            }
        }

        public DocumentTypeDefinition WebpageDefinition
        {
            get { return Webpage == null ? null : Webpage.GetDefinition(); }
        }

        private Webpage GetWebpage()
        {
            var data = Convert.ToString(RequestContext.RouteData.Values["data"]);

            var webpage = string.IsNullOrWhiteSpace(data)
                              ? !MrCMSApplication.UserLoggedIn
                                    ? MrCMSApplication.PublishedRootChildren(SiteSettings.Site).FirstOrDefault()
                                    : MrCMSApplication.RootChildren(SiteSettings.Site).FirstOrDefault()
                              : DocumentService.GetDocumentByUrl<Webpage>(data, _siteSettings.Site);

            MrCMSApplication.CurrentPage = webpage;
            _webpageLookedUp = true;
            return webpage;
        }

        bool IHttpHandler.IsReusable
        {
            get { return false; }
        }

        public ISession Session
        {
            get { return _session ?? (_session = _getSession.Invoke()); }
        }

        public IDocumentService DocumentService
        {
            get { return _documentService ?? (_documentService = GetDocumentService.Invoke()); }
        }

        public Func<IDocumentService> GetDocumentService
        {
            get { return _getDocumentService; }
        }

        public SiteSettings SiteSettings
        {
            get { return _siteSettings ?? (_siteSettings = GetSiteSettings.Invoke()); }
        }

        public Func<SiteSettings> GetSiteSettings
        {
            get { return _getSiteSettings; }
        }

        public RequestContext RequestContext
        {
            get { return _requestContext; }
        }
    }
}