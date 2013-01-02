using System.Web.Mvc;

namespace MrCMS.Website.Controllers
{
    public class MrCMSController : Controller
    {
        public ActionResult Process(string data)
        {
            return Redirect("~");
        }

        public string ReferrerOverride { get; set; }
        protected string Referrer
        {
            get { return ReferrerOverride ?? HttpContext.Request.UrlReferrer.ToString(); }
        }

        private TempDataDictionary _tempDataDictionary;
        public new TempDataDictionary TempData
        {
            get
            {
                if (ControllerContext != null && ControllerContext.IsChildAction)
                {
                    return ControllerContext.ParentActionViewContext.TempData;
                }
                return _tempDataDictionary ?? (_tempDataDictionary = new TempDataDictionary());
            }
        }
    }
}