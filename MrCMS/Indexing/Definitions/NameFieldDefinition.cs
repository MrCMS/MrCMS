using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;
using MrCMS.Website;

namespace MrCMS.Indexing.Definitions
{
    public class NameFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        public NameFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "name")
        {
        }

        protected override IEnumerable<string> GetValues(Webpage obj)
        {
            yield return obj.Name;
        }
    }
}