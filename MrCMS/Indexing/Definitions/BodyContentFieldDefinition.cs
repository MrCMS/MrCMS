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
        private static readonly Regex WhiteSpaceRegex = new Regex(@"\s{2,}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex TagStripRegex = new Regex(@"<[^>]+>|&nbsp;",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
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
            var trim = TagStripRegex.Replace(data ?? string.Empty, " ").Trim();
            return WhiteSpaceRegex.Replace(trim, " ");
        }
    }
}