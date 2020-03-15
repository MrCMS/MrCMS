using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;
#pragma warning disable 1998

namespace MrCMS.Indexing.Definitions
{
    public class TypeFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        public TypeFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "type")
        {
        }

        protected override async IAsyncEnumerable<string> GetValues(Webpage obj)
        {
            yield return obj.GetType().Name;
        }
    }
}