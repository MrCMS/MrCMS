using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Helpers
{
    public static class ArticleDisplayHelper
    {
        public static string DisplayHierarchicalName(this Document document)
        {
            List<Document> documents = GetParents(document).Reverse().ToList();

            return string.Join(" > ", documents.Select(doc => doc.Name));
        }

        private static IEnumerable<Document> GetParents(Document document)
        {
            Document page = document;
            while (page != null)
            {
                yield return page;
                page = page.Parent.Unproxy();
            }
        }
    }
}