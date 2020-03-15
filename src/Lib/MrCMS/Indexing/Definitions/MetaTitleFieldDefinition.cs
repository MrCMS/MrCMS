using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;
#pragma warning disable 1998

namespace MrCMS.Indexing.Definitions
{
    public class MetaTitleFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        public MetaTitleFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "metatitle")
        {
        }

        protected override async IAsyncEnumerable<string> GetValues(Webpage obj)
        {
            if (obj.MetaTitle != null) yield return obj.MetaTitle;
        }
    }
}