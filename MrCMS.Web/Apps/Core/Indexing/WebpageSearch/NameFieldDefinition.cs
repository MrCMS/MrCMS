using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class NameFieldDefinition : StringFieldDefinition<WebpageSearchIndexDefinition, Webpage>
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