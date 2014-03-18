using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class BodyContentFirstParagraphContainsTerm : BaseSEOAnalysisFacetProvider
    {
        public override IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            var paragraphs = document.GetElementsOfType("p").ToList();
            if (!paragraphs.Any())
            {
                yield return
                    GetFacet("Body Content", SEOAnalysisStatus.Error,
                        string.Format("Body content contains no paragraphs of text"));
                yield break;
            }
            if (paragraphs[0].InnerText.Contains(analysisTerm, StringComparison.OrdinalIgnoreCase))
            {
                yield return GetFacet("Body Content", SEOAnalysisStatus.Success,
                    string.Format("The first paragraph contains the term '{0}'", analysisTerm));
            }
            else
            {
                yield return GetFacet("Body Content", SEOAnalysisStatus.CanBeImproved,
                    string.Format("The first paragraph does not contain the term '{0}'", analysisTerm));
            }
        }
    }
}