using System.Collections.Generic;
using System.Text.RegularExpressions;
using Lucene.Net.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;

namespace MrCMS.Indexing.Definitions
{
    public class BodyContentFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        public BodyContentFieldDefinition(ILuceneSettingsService luceneSettingsService)
            : base(luceneSettingsService, "bodycontent")
        {
        }

        protected override IEnumerable<string> GetValues(Webpage obj)
        {
            yield return RemoveTags(obj.BodyContent);
        }

        private string RemoveTags(string data)
        {
            var trim = Regex.Replace(data ?? string.Empty, @"<[^>]+>|&nbsp;", " ").Trim();
            return Regex.Replace(trim, @"\s{2,}", " ");
        }
    }
}