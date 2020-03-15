using System.Collections.Generic;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;
#pragma warning disable 1998

namespace MrCMS.Indexing.Definitions
{
    public class CreatedOnFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        public CreatedOnFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "createdon")
        {
        }

        protected override async IAsyncEnumerable<string> GetValues(Webpage obj)
        {
            yield return DateTools.DateToString(obj.CreatedOn, DateTools.Resolution.SECOND);
        }
    }
}