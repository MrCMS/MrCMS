using System.Collections.Generic;
using System.Text.RegularExpressions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class BodyContentFieldDefinition : StringFieldDefinition<WebpageSearchIndexDefinition, Webpage>
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
            string trim = TagStripRegex.Replace(data ?? string.Empty, " ").Trim();
            return WhiteSpaceRegex.Replace(trim, " ");
        }
    }
}