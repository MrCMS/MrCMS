using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
#pragma warning disable 1998

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class MetaTitleFieldDefinition : StringFieldDefinition<WebpageSearchIndexDefinition, Webpage>
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