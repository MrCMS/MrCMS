using MrCMS.Helpers;

namespace MrCMS.Website.Controllers
{
    [HandleWebpageViews]
    [EnablePublishedFilter]
    public abstract class MrCMSUIController : MrCMSController
    {
        public void SetPageTitle(string pageTitle)
        {
            ViewData[MrCMSPageExtensions.PageTitleKey] = pageTitle;
        }

        public void SetPageMetaDescription(string pageDescription)
        {
            ViewData[MrCMSPageExtensions.PageDescriptionKey] = pageDescription;
        }

        public void SetPageMetaKeywords(string pageKeywords)
        {
            ViewData[MrCMSPageExtensions.PageKeywordsKey] = pageKeywords;
        }
    }
}