using System;
using MrCMS.Helpers;

namespace MrCMS.Website.Controllers
{
    [HandleWebpageViews]
    public abstract class MrCMSUIController : MrCMSController
    {
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
    }
}