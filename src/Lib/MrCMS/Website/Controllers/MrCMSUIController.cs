using Microsoft.AspNetCore.Mvc;
using MrCMS.Helpers;

namespace MrCMS.Website.Controllers
{
    [HandleWebpageViews]
    public abstract class MrCMSUIController : MrCMSController
    {
        [NonAction]
        public void SetPageTitle(string pageTitle)
        {
            ViewData[MrCMSPageExtensions.PageTitleKey] = pageTitle;
        }

        [NonAction]
        public void SetPageMetaDescription(string pageDescription)
        {
            ViewData[MrCMSPageExtensions.PageDescriptionKey] = pageDescription;
        }

        [NonAction]
        public void SetPageMetaKeywords(string pageKeywords)
        {
            ViewData[MrCMSPageExtensions.PageKeywordsKey] = pageKeywords;
        }
    }
}