using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class BodyContentMinimumWordCount : BaseSEOAnalysisFacetProvider
    {
        public override IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            var text =
                (HtmlNode.CreateNode("<div>" + webpage.BodyContent + "</div>").InnerText ?? string.Empty).Replace(
                    Environment.NewLine, " ");
            var strings = text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var wordCount = strings.Count();
            if (wordCount <= 300)
            {
                yield return
                    GetFacet("Word Count", SEOAnalysisStatus.Error,
                        string.Format(
                            "The word count is currently less than the 300 word recommended minimum ({0} words)",
                            wordCount));
            }
            else
            {
                yield return
                    GetFacet("Word Count", SEOAnalysisStatus.Success,
                        string.Format(
                            "The word count is currently at leastt the 300 word recommended minimum ({0} words)",
                            wordCount));
            }
        }
    }
}