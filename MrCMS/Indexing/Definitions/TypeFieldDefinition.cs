using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;

namespace MrCMS.Indexing.Definitions
{
    public class TypeFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        public TypeFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "type", index: Field.Index.NOT_ANALYZED)
        {
        }

        protected override IEnumerable<string> GetValues(Webpage obj)
        {
            yield return obj.GetType().Name;
        }
    }
}