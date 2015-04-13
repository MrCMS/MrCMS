using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;

namespace MrCMS.Indexing.Definitions
{
    public class MetaTitleFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        public MetaTitleFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "metatitle")
        {
        }

        protected override IEnumerable<string> GetValues(Webpage obj)
        {
            if (obj.MetaTitle != null) yield return obj.MetaTitle;
        }
    }
}