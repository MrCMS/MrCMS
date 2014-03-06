using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class TitleContainsTerm : BaseSEOAnalysisFacetProvider
    {
        public override IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            string titleText = document.GetElementText("title");
            yield return
                titleText.Contains(analysisTerm, StringComparison.OrdinalIgnoreCase)
                    ? GetFacet("Title contains term", SEOAnalysisStatus.Success, "The title contains '" + analysisTerm + "'")
                    : GetFacet("Title contains term", SEOAnalysisStatus.Error, "The title does not contain '" + analysisTerm + "'");
        }
    }
}