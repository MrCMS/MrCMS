using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.Resources;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class AllImagesHaveAltTags : BaseSEOAnalysisFacetProvider
    {
        private readonly IStringResourceProvider _stringResourceProvider;

        public AllImagesHaveAltTags(IStringResourceProvider stringResourceProvider)
        {
            _stringResourceProvider = stringResourceProvider;
        }

        public override IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            var images = document.GetElementsOfType("img").ToList();

            IEnumerable<HtmlNode> imagesWithoutAlt = images.FindAll(node => !node.Attributes.Contains("alt"));

            string hasNotGotAnAltTag = _stringResourceProvider.GetValue("Admin SEO Analysis Has not got", " has not got an alt tag");
            yield return GetFacet(_stringResourceProvider.GetValue("Admin SEO Analysis Has not got alt tags", "All images have alt tags"),
                !imagesWithoutAlt.Any()
                    ? SEOAnalysisStatus.Success
                    : SEOAnalysisStatus.Error,
                imagesWithoutAlt.Select(node => node.GetAttributeValue("src", "") + hasNotGotAnAltTag).ToArray());

        }
    }
}