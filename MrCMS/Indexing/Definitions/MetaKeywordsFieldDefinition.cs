using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;

namespace MrCMS.Indexing.Definitions
{
    public class MetaKeywordsFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        public MetaKeywordsFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "metakeywords")
        {
        }

        protected override IEnumerable<string> GetValues(Webpage obj)
        {
            yield return obj.MetaKeywords;
        }
    }
}