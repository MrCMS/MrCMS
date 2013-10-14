using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website.Optimization;
using NHibernate;

namespace MrCMS.Website.Routing
{
    public abstract class ErrorHandlingHttpHandler : IHttpHandler, IRequiresSessionState
    {
        private readonly IDocumentService _documentService;
        private readonly IControllerManager _controllerManager;

        protected ErrorHandlingHttpHandler(IDocumentService documentService, IControllerManager controllerManager)
        {
            _documentService = documentService;
            _controllerManager = controllerManager;
        }

        public abstract void ProcessRequest(HttpContext context);
        public abstract bool IsReusable { get; }


        private void HandleExceptionWithElmah(Exception exception)
        {
            CurrentRequestData.ErrorSignal.Raise(exception);
        }

        protected void HandleError(HttpContextBase context, int code, int pageId, HttpException exception)
        {
            HandleExceptionWithElmah(exception);
            var webpage = _documentService.GetDocument<Webpage>(pageId);
            if (webpage != null)
            {
                context.ClearError();
                context.Response.Clear();
                context.Response.StatusCode = code;
                context.Response.TrySkipIisCustomErrors = true;

                var data = new RouteData();
                data.Values["data"] = webpage.LiveUrlSegment;

                CurrentRequestData.CurrentPage = webpage;
                var controller = _controllerManager.GetController(RequestContext, webpage, context.Request.HttpMethod);
                (controller as IController).Execute(new RequestContext(context, controller.RouteData));
            }
            else
            {
                throw exception;
            }
        }

        public RequestContext RequestContext { get; private set; }
        public void SetRequestContext(RequestContext requestContext)
        {
            RequestContext = requestContext;
        }
    }

    public sealed class MrCMSHttpHandler : ErrorHandlingHttpHandler
    {
        private readonly IDocumentService _documentService;
        private readonly SiteSettings _siteSettings;
        private readonly SEOSettings _seoSettings;
        private Webpage _webpage;
        private bool _webpageLookedUp;
        private readonly ISession _session;
        private readonly IControllerManager _controllerManager;


        public MrCMSHttpHandler(ISession session, IDocumentService documentService, IControllerManager controllerManager,
                                SiteSettings siteSettings, SEOSettings seoSettings)
            : base(documentService, controllerManager)
        {
            _session = session;
            _documentService = documentService;
            _controllerManager = controllerManager;
            _siteSettings = siteSettings;
            _seoSettings = seoSettings;
        }

        public override void ProcessRequest(HttpContext context)
        {
            // Wrapped up to aid testing
            ProcessRequest(new HttpContextWrapper(context));
        }

        public void ProcessRequest(HttpContextBase context)
        {
            SetCustomHeaders(context);

            if (CheckIsFile(context)) return;

            if (HandleUrlHistory(context)) return;

            if (Handle404(context)) return;

            if (PageIsRedirect(context)) return;

            if (RequiresSSLRedirect(context)) return;

            if (RedirectsToHomePage(context)) return;

            if (!IsAllowed(context)) return;

            var controller = _controllerManager.GetController(RequestContext, Webpage, context.Request.HttpMethod);

            _controllerManager.SetViewData(Webpage, controller, _session);

            _controllerManager.SetFormData(Webpage, controller, context.Request.Form);

            try
            {
                if (_seoSettings.EnableHtmlMinification)
                    context.Response.Filter = new WhitespaceFilter(context.Response.Filter);
                (controller as IController).Execute(RequestContext);
            }
            catch (Exception exception)
            {
                Handle500(context, exception);
            }
        }

        private bool HandleUrlHistory(HttpContextBase context)
        {
            if (Webpage == null)
            {
                var historyItemByUrl = _documentService.GetHistoryItemByUrl(Data);
                if (historyItemByUrl != null)
                {
                    context.Response.RedirectPermanent("~/" + historyItemByUrl.Webpage.LiveUrlSegment);
                    return true;
                }
                if (context.Request.Url != null)
                {
                    var historyItemByUrlContent = _documentService.GetHistoryItemByUrl(context.Request.Url.PathAndQuery.TrimStart('/'));
                    if (historyItemByUrlContent != null)
                    {
                        context.Response.RedirectPermanent("~/" + historyItemByUrlContent.Webpage.LiveUrlSegment);
                        return true;
                    }
                }
            }
            return false;
        }

        private void SetCustomHeaders(HttpContextBase context)
        {
            context.Response.AppendHeader("X-Built-With", "Mr CMS - http://www.mrcms.com");
        }

        public void Handle500(HttpContextBase context, Exception exception)
        {
            HandleError(context, 500, _siteSettings.Error500PageId, new HttpException(500, exception.Message, exception));
        }

        public bool IsAllowed(HttpContextBase context)
        {
            if (!Webpage.IsAllowed(CurrentRequestData.CurrentUser))
            {
                if (CurrentRequestData.CurrentUser != null)
                    Handle403(context);
                else
                    HandleError(context, 401, _siteSettings.Error403PageId, new HttpException(401, "Not allowed to view " + Data));
                return false;
            }
            return true;
        }

        public void Handle403(HttpContextBase context)
        {
            HandleError(context, 403, _siteSettings.Error403PageId, new HttpException(403, "Not allowed to view " + Data));
        }

        public bool Handle404(HttpContextBase context)
        {
            if (Webpage == null || (!Webpage.Published) && !CurrentRequestData.CurrentUserIsAdmin)
            {
                HandleError(context, 404, _siteSettings.Error404PageId, new HttpException(404, "Cannot find " + Data));
                return true;
            }
            return false;
        }

        public bool RequiresSSLRedirect(HttpContextBase context)
        {
            var url = context.Request.Url;
            var scheme = url.Scheme;
            if (CurrentRequestData.CurrentUserIsAdmin && _siteSettings.SSLAdmin && _siteSettings.SiteIsLive && !context.Request.IsLocal)
            {
                if (scheme == "http")
                {
                    var redirectUrl = url.ToString().Replace(scheme + "://", "https://");
                    context.Response.Redirect(redirectUrl);
                    return true;
                }
                return false;
            }
            if (CurrentRequestData.CurrentUserIsAdmin && scheme == "http" && _siteSettings.SSLAdmin && _siteSettings.SiteIsLive && !context.Request.IsLocal)
            {
                var redirectUrl = url.ToString().Replace(scheme + "://", "https://");
                context.Response.Redirect(redirectUrl);
                return true;
            }
            if (Webpage.RequiresSSL && scheme != "https" && _siteSettings.SiteIsLive)
            {
                var redirectUrl = url.ToString().Replace(scheme + "://", "https://");
                context.Response.RedirectPermanent(redirectUrl);
                return true;
            }
            if (!Webpage.RequiresSSL && scheme != "http")
            {
                var redirectUrl = url.ToString().Replace(scheme + "://", "http://");
                context.Response.RedirectPermanent(redirectUrl);
                return true;
            }
            return false;
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

        public bool PageIsRedirect(HttpContextBase context)
        {
            if (Webpage is Redirect)
            {
                var redirect = (Webpage as Redirect);
                string redirectUrl = redirect.RedirectUrl;
                Uri result;
                if (Uri.TryCreate(redirectUrl, UriKind.Absolute, out result))
                {
                    if (redirect.Permanent)
                        context.Response.RedirectPermanent(redirectUrl);
                    else
                        context.Response.Redirect(redirectUrl);
                }
                else
                {
                    if (redirectUrl.StartsWith("/"))
                        redirectUrl = redirectUrl.Substring(1);

                    if (redirect.Permanent)
                        context.Response.RedirectPermanent("~/" + redirectUrl);
                    else
                        context.Response.Redirect("~/" + redirectUrl);
                }
                return true;
            }
            return false;
        }

        public bool CheckIsFile(HttpContextBase context)
        {
            var path = RequestContext.HttpContext.Request.Url.AbsolutePath;
            var extension = Path.GetExtension(path);
            if (!string.IsNullOrWhiteSpace(extension))
            {
                context.Response.Clear();
                context.Response.StatusCode = 404;
                context.Response.End();
                return true;
            }
            return false;
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

        private Webpage GetWebpage()
        {
            Webpage webpage = string.IsNullOrWhiteSpace(Data)
                                  ? CurrentRequestData.HomePage
                                  : _documentService.GetDocumentByUrl<Webpage>(Data);

            CurrentRequestData.CurrentPage = webpage;
            _webpageLookedUp = true;

            if (webpage != null)
            {
                if (MrCMSApp.AppWebpages.ContainsKey(webpage.GetType()))
                    RequestContext.RouteData.DataTokens["app"] = MrCMSApp.AppWebpages[webpage.GetType()];
            }
            return webpage;
        }

        private string Data
        {
            get { return Convert.ToString(RequestContext.RouteData.Values["data"]); }
        }


        public override bool IsReusable
        {
            get { return false; }
        }
    }
}