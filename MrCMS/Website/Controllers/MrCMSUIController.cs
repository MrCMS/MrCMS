using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
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

        protected void SetPageTitle(string pageTitle)
        {
            ViewData[MrCMSPageExtensions.PageTitleKey] = pageTitle;
        }

        protected void SetPageMetaDescription(string pageDescription)
        {
            ViewData[MrCMSPageExtensions.PageDescriptionKey] = pageDescription;
        }

        protected void SetPageMetaKeywords(string pageKeywords)
        {
            ViewData[MrCMSPageExtensions.PageKeywordsKey] = pageKeywords;
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            if (!(model is Webpage))
                return base.View(viewName, masterName, model);

            var webpage = model as Webpage;
            if (string.IsNullOrWhiteSpace(viewName))
            {
                if (webpage.PageTemplate != null && !string.IsNullOrWhiteSpace(webpage.PageTemplate.PageTemplateName))
                {
                    viewName = webpage.PageTemplate.PageTemplateName;
                }
                else
                {
                    viewName = model.GetType().Name;
                }
            }

            if (string.IsNullOrWhiteSpace(masterName))
            {
                Layout layout = GetCurrentLayout.Get(webpage);
                if (layout != null)
                {
                    masterName = layout.GetLayoutName();
                }
            }

            return base.View(viewName, masterName, model);
        }
    }
}