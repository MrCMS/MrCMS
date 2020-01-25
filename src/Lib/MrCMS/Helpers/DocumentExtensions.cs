using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class DocumentExtensions
    {
        public static bool CanDeleteDocument(this IHtmlHelper helper, int id)
        {
            return !helper.AnyChildren(id);
        }

        public static bool AnyChildren(this IHtmlHelper helper, int id)
        {
            var document = helper.GetRequiredService<IRepository<Document>>().GetDataSync<Document>(id);
            if (document == null)
                return false;
            return AnyChildren(helper, document);
        }

        private static bool AnyChildren(this IHtmlHelper helper, Document document)
        {
            return helper.GetRequiredService<IRepository<Document>>()
                .Query()
                //.Cacheable()
                .Any(doc => doc.Parent != null && doc.Parent.Id == document.Id);
        }

        public static bool CanDelete<T>(this IHtmlHelper<T> helper) where T : Document
        {
            return !helper.AnyChildren(helper.ViewData.Model);
        }

        public static bool AnyChildren<T>(this IHtmlHelper<T> helper) where T : Document
        {
            return helper.AnyChildren(helper.ViewData.Model);
        }

        public static string GetAdminController(this Document document)
        {
            return document is Layout ? "Layout" : document is MediaCategory ? "MediaCategory" : "Webpage";
        }

    }
}