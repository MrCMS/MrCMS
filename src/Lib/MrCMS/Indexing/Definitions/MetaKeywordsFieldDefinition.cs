using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;
#pragma warning disable 1998

namespace MrCMS.Indexing.Definitions
{
    public class MetaKeywordsFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        public MetaKeywordsFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "metakeywords")
        {
        }

        protected override async IAsyncEnumerable<string> GetValues(Webpage obj)
        {
            if (obj.MetaKeywords != null) yield return obj.MetaKeywords;
        }
    }
}