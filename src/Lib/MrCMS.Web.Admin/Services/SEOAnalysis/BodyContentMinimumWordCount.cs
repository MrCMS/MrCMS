using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public class BodyContentMinimumWordCount : BaseSEOAnalysisFacetProvider
    {
        public override Task<IReadOnlyList<SEOAnalysisFacet>> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var text =
                (HtmlNode.CreateNode("<div>" + webpage.BodyContent + "</div>").InnerText ?? string.Empty).Replace(
                    Environment.NewLine, " ");
            var strings = text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var wordCount = strings.Count();
            var result = new List<SEOAnalysisFacet>();
            if (wordCount <= 300)
            {
                result.Add(
                    GetFacet("Word Count", SEOAnalysisStatus.Error,
                        $"The word count is currently less than the 300 word recommended minimum ({wordCount} words)"));
            }
            else
            {
                result.Add(
                    GetFacet("Word Count", SEOAnalysisStatus.Success,
                        $"The word count is currently at leastt the 300 word recommended minimum ({wordCount} words)"));
            }
            return Task.FromResult<IReadOnlyList<SEOAnalysisFacet>>(result);
        }
    }
}