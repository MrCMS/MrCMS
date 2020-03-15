using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
#pragma warning disable 1998

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class DisplayOrderFieldDefinition : IntegerFieldDefinition<WebpageSearchIndexDefinition, Webpage>
    {
        public DisplayOrderFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "displayorder")
        {
        }

        protected override async IAsyncEnumerable<int> GetValues(Webpage obj)
        {
            yield return obj.DisplayOrder;
        }
    }
}