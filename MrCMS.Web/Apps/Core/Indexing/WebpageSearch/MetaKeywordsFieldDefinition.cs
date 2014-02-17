using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class MetaKeywordsFieldDefinition : StringFieldDefinition<WebpageSearchIndexDefinition, Webpage>
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