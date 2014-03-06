using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class MetaDescriptionFieldDefinition : StringFieldDefinition<WebpageSearchIndexDefinition, Webpage>
    {
        public MetaDescriptionFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "metadescription")
        {
        }

        protected override IEnumerable<string> GetValues(Webpage obj)
        {
            yield return obj.MetaDescription;
        }
    }
}