using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Apps.Admin.Services.SEOAnalysis
{
    public class KeywordDensity : BaseSEOAnalysisFacetProvider
    {
        public override async IAsyncEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var text =
                (HtmlNode.CreateNode("<div>" + webpage.BodyContent + "</div>").InnerText ?? string.Empty).Replace(
                    Environment.NewLine, " ");

            if (text.Count() < 10)
            {
                yield return
                    GetFacet("Keyword density", SEOAnalysisStatus.Error, "There is not enough content to calculate keyword density.");
            }

            var instances = Regex.Matches(text, analysisTerm, RegexOptions.IgnoreCase).Count;

            var strings = text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var words = strings.Count();
            if (words == 0)
                yield break;
            var termWordCount = analysisTerm.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Count();

            var density = ((decimal)instances / (words - (termWordCount * (termWordCount - 1)))) * 100m;
            // (instances / (words - (instances * (decimal)termWordCount)) * 100m);

            if (density < 1)
            {
                yield return
                    GetFacet("Keyword density", SEOAnalysisStatus.Error,
                        $"The current keyword density in your body content is {density:0.00}% , which is below the recommended 1-4.5% target.");
            }
            else if (density > 4.5m)
            {
                yield return
                    GetFacet("Keyword density", SEOAnalysisStatus.Error,
                        $"The current keyword density in your body content is {density:0.00}% , which is above the recommended 1-4.5% target.");
            }
            else
            {
                yield return
                    GetFacet("Keyword density", SEOAnalysisStatus.Success,
                        $"The current keyword density in your body content is {density:0.00}% , which is within the recommended 1-4.5% target.");

            }
        }
    }
}