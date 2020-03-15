using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
#pragma warning disable 1998

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class NameFieldDefinition : StringFieldDefinition<WebpageSearchIndexDefinition, Webpage>
    {
        public NameFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "name")
        {
        }

        protected override async IAsyncEnumerable<string> GetValues(Webpage obj)
        {
            if (obj.Name != null) yield return obj.Name;
        }
    }
}