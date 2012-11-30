using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    [ValidateInput(false)]
    public abstract class AdminController : Controller
    {
        private bool? _isAjaxRequest;

        public bool IsAjaxRequest
        {
            get { return _isAjaxRequest.HasValue ? _isAjaxRequest.Value : Request.IsAjaxRequest(); }
            set { _isAjaxRequest = value; }
        }

        protected new HttpRequestBase Request
        {
            get { return RequestMock ?? base.Request; }
        }

        public HttpRequestBase RequestMock { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["controller-name"] = ControllerContext.RouteData.Values["controller"];
            base.OnActionExecuting(filterContext);
        }

        protected new ViewResultBase View(object model)
        {
            return IsAjaxRequest ? (ViewResultBase)PartialView(model) : base.View(model);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding,
                                           JsonRequestBehavior behavior)
        {
            return base.Json(data, contentType, contentEncoding, JsonRequestBehavior.AllowGet);
        }

        protected string GetReferrer
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(GetReferrerOverride))
                    return GetReferrerOverride;
                if (HttpContext != null && HttpContext.Request != null && HttpContext.Request.UrlReferrer != null)
                    return HttpContext.Request.UrlReferrer.ToString();
                return null;
            }
        }

        public string GetReferrerOverride { get; set; }

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

        public class JsonNetResult : JsonResult
        {
            public JsonNetResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet;
            }

            public Encoding ContentEncoding { get; set; }
            public string ContentType { get; set; }
            public object Data { get; set; }
            public string JsonData { get; set; }
            public JsonRequestBehavior JsonRequestBehavior { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                HttpResponseBase response = context.HttpContext.Response;

                response.ContentType = !String.IsNullOrEmpty(ContentType)
                                           ? ContentType
                                           : "application/json";
                if (ContentEncoding != null)
                    response.ContentEncoding = ContentEncoding;

                if (!string.IsNullOrWhiteSpace(JsonData))
                {
                    response.Write(JsonData);
                    return;
                }
                if (Data == null) return;

                var serializedData = Newtonsoft.Json.JsonConvert.SerializeObject(Data);
                response.Write(serializedData);
            }
        }
    }
}