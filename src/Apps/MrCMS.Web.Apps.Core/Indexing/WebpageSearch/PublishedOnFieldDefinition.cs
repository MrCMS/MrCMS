using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
#pragma warning disable 1998

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class PublishedOnFieldDefinition : StringFieldDefinition<WebpageSearchIndexDefinition, Webpage>
    {
        public PublishedOnFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "publishon")
        {
        }

        protected override async IAsyncEnumerable<string> GetValues(Webpage obj)
        {
            yield return
                DateTools.DateToString(obj.PublishOn.GetValueOrDefault(DateTime.MaxValue), DateTools.Resolution.SECOND);
        }
    }
}