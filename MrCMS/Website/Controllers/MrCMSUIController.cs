using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Services;

namespace MrCMS.Website.Controllers
{
    public abstract class MrCMSUIController : MrCMSController
    {
        private IGetCurrentLayout _getCurrentLayout;

        public IGetCurrentLayout GetCurrentLayout
        {
            get { return _getCurrentLayout ?? MrCMSApplication.Get<IGetCurrentLayout>(); }
            set { _getCurrentLayout = value; }
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            if (!(model is Webpage) && !(model is Widget))
                return base.View(viewName, masterName, model);

            if (string.IsNullOrWhiteSpace(viewName))
                viewName = model.GetType().Name;

            if (string.IsNullOrWhiteSpace(masterName) && model is Webpage)
            {
                var layout = GetCurrentLayout.Get(model as Webpage);
                if (layout != null)
                {
                    masterName = layout.UrlSegment;
                }
            }

            return base.View(viewName, masterName, model);
        }
    }
}