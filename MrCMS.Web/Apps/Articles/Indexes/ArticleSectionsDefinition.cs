using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Indexes
{
    public class ArticleSectionsDefinition : StringFieldDefinition<ArticleIndexDefinition, Article>
    {
        public ArticleSectionsDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "section", Field.Store.YES, Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Article obj)
        {
            return ArticleSections(obj).Distinct().Select(section => section.Id.ToString());
        }

        private static IEnumerable<ArticleSection> ArticleSections(Article obj)
        {
            var webpage = obj.Parent as Webpage;
            foreach (var section in webpage.ActivePages.OfType<ArticleSection>())
            {
                yield return section;
            }
            foreach (var section in obj.OtherSections)
            {
                foreach (var hierarchy in section.ActivePages.OfType<ArticleSection>())
                {
                    yield return hierarchy;
                }
            }
        }
    }
}