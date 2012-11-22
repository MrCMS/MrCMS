using System.Linq;
using System.Web.Mvc;
using NHibernate;

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

        protected override void ExecuteCore()
        {
            // If code in this method needs to be updated, please also check the BeginExecuteCore() and
            // EndExecuteCore() methods of AsyncController to see if that code also must be updated.

            PossiblyLoadTempData();
            try
            {
                string actionName = RouteData.GetRequiredString("action");
                
                if (TempData.Any())
                    ControllerContext.Controller.TempData = TempData;

                if (!ActionInvoker.InvokeAction(ControllerContext, actionName))
                {
                    HandleUnknownAction(actionName);
                }
            }
            finally
            {
                PossiblySaveTempData();
            }
        }
        internal void PossiblyLoadTempData()
        {
            if (!ControllerContext.IsChildAction)
            {
                TempData.Load(ControllerContext, TempDataProvider);
            }
        }
        internal void PossiblySaveTempData()
        {
            if (!ControllerContext.IsChildAction)
            {
                TempData.Save(ControllerContext, TempDataProvider);
            }
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