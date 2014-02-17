using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class DisplayOrderFieldDefinition : IntegerFieldDefinition<WebpageSearchIndexDefinition, Webpage>
    {
        public DisplayOrderFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "displayorder", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<int> GetValues(Webpage obj)
        {
            yield return obj.DisplayOrder;
        }
    }
}