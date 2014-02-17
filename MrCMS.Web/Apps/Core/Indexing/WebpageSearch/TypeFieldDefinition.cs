using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class TypeFieldDefinition : StringFieldDefinition<WebpageSearchIndexDefinition, Webpage>
    {
        public TypeFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "type")
        {
        }

        protected override IEnumerable<string> GetValues(Webpage obj)
        {
            yield return obj.GetType().Name;
        }
    }
}