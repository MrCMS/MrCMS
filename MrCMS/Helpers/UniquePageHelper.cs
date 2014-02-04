using System;
using System.Linq;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public class UniquePageHelper
    {
        public static string GetUrl<T>(object queryString = null) where T : Webpage, IUniquePage
        {
            return Get<T>(queryString, arg => "/" + arg.LiveUrlSegment);
        }

        public static string GetAbsoluteUrl<T>(object queryString = null) where T : Webpage, IUniquePage
        {
            return Get<T>(queryString, arg => arg.AbsoluteUrl);
        }

        private static string Get<T>(object queryString, Func<T, string> selector) where T : Webpage, IUniquePage
        {
            var documentService = MrCMSApplication.Get<IDocumentService>();
            return documentService.RedirectTo<T>(queryString).Url;
        }
    }
}