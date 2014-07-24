using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Indexes
{
    public class ArticlePrimarySectionDefinition : StringFieldDefinition<ArticleIndexDefinition, Article>
    {
        public ArticlePrimarySectionDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "primarysection", Field.Store.YES, Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Article obj)
        {
            yield return obj.Parent.Id.ToString();
        }
    }
}