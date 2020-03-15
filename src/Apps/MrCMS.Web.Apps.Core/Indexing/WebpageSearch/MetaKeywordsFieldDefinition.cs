using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
#pragma warning disable 1998

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class MetaKeywordsFieldDefinition : StringFieldDefinition<WebpageSearchIndexDefinition, Webpage>
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