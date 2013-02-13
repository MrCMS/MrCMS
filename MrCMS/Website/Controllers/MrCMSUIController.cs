using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Website.Controllers
{
    public abstract class MrCMSUIController : MrCMSController
    {
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
}