using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class MrCMSPageExtensions
    {
        internal const string PageTitleKey = "page-title";
        internal const string PageDescriptionKey = "page-description";
        internal const string PageKeywordsKey = "page-keywords";
        public static string PageTitle<T>(this MrCMSPage<T> page) where T : Webpage
        {
            return page.ViewData[PageTitleKey] as string ?? page.Model.GetPageTitle();
        }

        public static string Description<T>(this MrCMSPage<T> page) where T : Webpage
        {
            return page.ViewData[PageDescriptionKey] as string ?? page.Model.MetaDescription;
        }

        public static string Keywords<T>(this MrCMSPage<T> page) where T : Webpage
        {
            return page.ViewData[PageKeywordsKey] as string ?? page.Model.MetaKeywords;
        }

        public static string GetPageTitle<T>(this T webpage) where T : Webpage
        {
            return !string.IsNullOrWhiteSpace(webpage.MetaTitle) ? webpage.MetaTitle : webpage.Name;
        }
    }
}