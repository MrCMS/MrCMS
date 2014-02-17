using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class MetaTitleFieldDefinition : StringFieldDefinition<WebpageSearchIndexDefinition, Webpage>
    {
        public MetaTitleFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "metatitle")
        {
        }

        protected override IEnumerable<string> GetValues(Webpage obj)
        {
            yield return obj.MetaTitle;
        }
    }
}