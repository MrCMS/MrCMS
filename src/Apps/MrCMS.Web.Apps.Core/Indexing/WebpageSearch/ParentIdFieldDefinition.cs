using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class ParentIdFieldDefinition : StringFieldDefinition<WebpageSearchIndexDefinition, Webpage>
    {
        public ParentIdFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "parentid")
        {
        }

        protected override async IAsyncEnumerable<string> GetValues(Webpage obj)
        {
            yield return obj.ParentId.ToString();
        }
    }
}