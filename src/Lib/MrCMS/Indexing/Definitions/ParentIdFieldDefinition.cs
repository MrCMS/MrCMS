using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;

namespace MrCMS.Indexing.Definitions
{
    public class ParentIdFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        public ParentIdFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "parentid")
        {
        }

        protected override async IAsyncEnumerable<string> GetValues(Webpage obj)
        {
            yield return obj.ParentId.ToString();
        }
    }
}