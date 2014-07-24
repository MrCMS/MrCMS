using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Indexes
{
    public class ArticlePublishedDefinition : StringFieldDefinition<ArticleIndexDefinition, Article>
    {
        public ArticlePublishedDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "publishedon", Field.Store.YES, Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Article obj)
        {
            yield return DateTools.DateToString(obj.PublishOn ?? DateTime.MaxValue, DateTools.Resolution.SECOND);
        }
    }
}