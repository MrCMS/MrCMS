using Microsoft.AspNetCore.Mvc.Razor;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Helpers
{
    public static class MrCMSPageExtensions
    {
        public const string PageTitleKey = "page-title";
        public const string PageDescriptionKey = "page-description";
        public const string PageKeywordsKey = "page-keywords";

        public static string PageTitle<T>(this RazorPage<T> page)
        {
            return page.ViewData[PageTitleKey] as string ?? (page.Model as Webpage)?.GetPageTitle();
        }

        public static string Description<T>(this RazorPage<T> page)
        {
            return page.ViewData[PageDescriptionKey] as string ?? (page.Model as Webpage)?.MetaDescription;
        }

        public static string Keywords<T>(this RazorPage<T> page)
        {
            return page.ViewData[PageKeywordsKey] as string ?? (page.Model as Webpage)?.MetaKeywords;
        }

        public static string GetPageTitle(this Webpage webpage)
        {
            return !string.IsNullOrWhiteSpace(webpage.MetaTitle) ? webpage.MetaTitle : webpage.Name;
        }
    }
}