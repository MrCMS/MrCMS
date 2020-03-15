using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;
#pragma warning disable 1998

namespace MrCMS.Indexing.Definitions
{
    public class MetaDescriptionFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        public MetaDescriptionFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "metadescription")
        {
        }

        protected override async IAsyncEnumerable<string> GetValues(Webpage obj)
        {
            if (obj.MetaDescription != null) yield return obj.MetaDescription;
        }
    }
}