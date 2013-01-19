using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;

namespace MrCMS.Website.Controllers
{
    [MrCMSAuthorize(Roles = UserRole.Administrator)]
    [ValidateInput(false)]
    public abstract class AdminController : Controller
    {
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

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CheckCurrentSite(filterContext);
            ViewData["controller-name"] = ControllerContext.RouteData.Values["controller"];
            base.OnActionExecuting(filterContext);
        }

        private void CheckCurrentSite(ActionExecutingContext filterContext)
        {
            var entities = filterContext.ActionParameters.Values.OfType<SiteEntity>();

            if (entities.Any(entity => entity.Website != CurrentSite && entity.Id != 0))
            {
                filterContext.Result = new RedirectResult("~/admin");
            }
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding,
                                           JsonRequestBehavior behavior)
        {
            return base.Json(data, contentType, contentEncoding, JsonRequestBehavior.AllowGet);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            return new JsonNetResult
            {
                ContentEncoding = contentEncoding,
                ContentType = contentType,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = data
            };
        }
        protected JsonResult Json(string data)
        {
            return new JsonNetResult
            {
                ContentEncoding = null,
                ContentType = null,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                JsonData = data
            };
        }
    }
}