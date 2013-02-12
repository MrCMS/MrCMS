using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Widget;

namespace MrCMS.Website.Controllers
{
    public abstract class MrCMSUIController : MrCMSController
    {
        private readonly string _appName;

        protected MrCMSUIController()
            : this(null)
        {
        }

        protected MrCMSUIController(string appName)
        {
            _appName = appName;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrWhiteSpace(_appName))
                RouteData.DataTokens["app"] = _appName;
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            if (!(model is Webpage) && !(model is Widget))
                return base.View(viewName, masterName, model);

            if (string.IsNullOrWhiteSpace(viewName))
                viewName = model.GetType().Name;

            if (string.IsNullOrWhiteSpace(masterName) && model is Webpage)
                masterName = (model as Webpage).CurrentLayout.UrlSegment;

            return base.View(viewName, masterName, model);
        }
    }
    public abstract class MrCMSController : Controller
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CheckCurrentSite(filterContext);
            base.OnActionExecuting(filterContext);
        }

        private void CheckCurrentSite(ActionExecutingContext filterContext)
        {
            var entities = filterContext.ActionParameters.Values.OfType<SiteEntity>();

            if (entities.Any(entity => entity.Site != CurrentSite && entity.Id != 0))
            {
                filterContext.Result = RedirectResult();
            }
        }
        public string ReferrerOverride { get; set; }
        protected string Referrer
        {
            get { return ReferrerOverride ?? HttpContext.Request.UrlReferrer.ToString(); }
        }

        private Site _currentSite;

        public Site CurrentSite
        {
            get { return _currentSite ?? MrCMSApplication.CurrentSite; }
            set { _currentSite = value; }
        }

        public new HttpRequestBase Request
        {
            get { return RequestMock ?? base.Request; }
        }

        public HttpRequestBase RequestMock { get; set; }

        protected virtual RedirectResult RedirectResult()
        {
            return Redirect("~");
        }
    }
}