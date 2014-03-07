using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class KeywordDensity : BaseSEOAnalysisFacetProvider
    {
        public override IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            var paragraphs = document.GetElementsOfType("p").ToList();

            var text = string.Join(" ", paragraphs.Select(node => node.InnerText.Replace(Environment.NewLine, " ")));

            var instances = Regex.Matches(text, analysisTerm, RegexOptions.IgnoreCase).Count;

            var words = text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Count();
            var termWordCount = analysisTerm.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Count();

            var density = (instances / (words / (decimal)termWordCount)) * 100m;

            if (density < 1)
            {
                yield return
                    GetFacet("Keyword density", SEOAnalysisStatus.Error,
                        string.Format(
                            "The current keyword density in your body content is {0:0.00}% , which is below the recommended 1-4.5% target.",
                            density));
            }
            else if (density > 4.5m)
            {
                yield return
                    GetFacet("Keyword density", SEOAnalysisStatus.Error,
                        string.Format(
                            "The current keyword density in your body content is {0:0.00}% , which is above the recommended 1-4.5% target.",
                            density));
            }
            else
            {
                yield return
                    GetFacet("Keyword density", SEOAnalysisStatus.Success,
                        string.Format(
                            "The current keyword density in your body content is {0:0.00}% , which is within the recommended 1-4.5% target.",
                            density));

            }
        }
    }
}