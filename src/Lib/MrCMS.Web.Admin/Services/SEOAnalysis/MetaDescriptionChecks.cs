using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models.SEOAnalysis;
using MrCMS.Web.Admin.Helpers;
using NHibernate;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public class MetaDescriptionChecks : BaseSEOAnalysisFacetProvider
    {
        private readonly ISession _session;

        public MetaDescriptionChecks(ISession session)
        {
            _session = session;
        }

        public override async Task<IReadOnlyList<SEOAnalysisFacet>> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var descriptionElement = document.ChildNodesRecursive().FirstOrDefault(node =>
                node.Name == "meta" && node.GetAttributeValue("name", "") == "description");
            string metaDescription = descriptionElement != null
                ? descriptionElement.GetAttributeValue("content", "")
                : string.Empty;
            if (string.IsNullOrWhiteSpace(metaDescription))
            {
                return new List<SEOAnalysisFacet>
                {
                    GetFacet("Meta description set", SEOAnalysisStatus.Error,
                        "No meta description has been set - search engines will instead use text from your copy in results")
                };
            }

            var result = new List<SEOAnalysisFacet>
            {
                GetFacet("Meta description set", SEOAnalysisStatus.Success, "Meta description is set"),
                metaDescription.Contains(analysisTerm, StringComparison.OrdinalIgnoreCase)
                    ? GetFacet("Meta description contains analysis term", SEOAnalysisStatus.Success,
                        $"Meta description contains '{analysisTerm}'")
                    : GetFacet("Meta description contains analysis term", SEOAnalysisStatus.Error,
                        $"Meta description does not contain '{analysisTerm}'")
            };
            if (metaDescription.Length < 120)
            {
                result.Add(
                    GetFacet("Meta description length", SEOAnalysisStatus.CanBeImproved,
                        "Meta description should be at least 120 characters"));
            }
            else if (metaDescription.Length > 200)
            {
                result.Add(
                    GetFacet("Meta description length", SEOAnalysisStatus.Error,
                        "Meta description should be at most 200 characters"));
            }
            else
            {
                result.Add(
                    GetFacet("Meta description length", SEOAnalysisStatus.Success,
                        "Meta description is of optimal length (between 120 and 200 characters)"));
            }

            bool anyWithSameDescription =
                await _session.QueryOver<Webpage>()
                    .Where(page =>
                        page.Site.Id == webpage.Site.Id && page.Id != webpage.Id &&
                        page.MetaDescription == webpage.MetaDescription)
                    .AnyAsync();

            if (anyWithSameDescription)
                result.Add(
                    GetFacet("Any other pages with same meta description?", SEOAnalysisStatus.Error,
                        "The meta description is not unique"));
            else
                result.Add(
                    GetFacet("Any other pages with same meta description?", SEOAnalysisStatus.Success,
                        "The meta description is unique"));
            return result;
        }
    }
}