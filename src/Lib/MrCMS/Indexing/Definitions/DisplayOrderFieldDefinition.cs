using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;
#pragma warning disable 1998

namespace MrCMS.Indexing.Definitions
{
    public class DisplayOrderFieldDefinition : IntegerFieldDefinition<AdminWebpageIndexDefinition, Webpage>
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