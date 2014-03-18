using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class MetaDescriptionChecks : BaseSEOAnalysisFacetProvider
    {
        private readonly ISession _session;

        public MetaDescriptionChecks(ISession session)
        {
            _session = session;
        }

        public override IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            var descriptionElement = document.ChildNodesRecursive().FirstOrDefault(node => node.Name == "meta" && node.GetAttributeValue("name","") == "description");
            string metaDescription = descriptionElement != null
                ? descriptionElement.GetAttributeValue("content", "")
                : string.Empty;
            if (string.IsNullOrWhiteSpace(metaDescription))
            {
                yield return GetFacet("Meta description set", SEOAnalysisStatus.Error, "No meta description has been set - search engines will instead use text from your copy in results");
                yield break;
            }
            yield return GetFacet("Meta description set", SEOAnalysisStatus.Success, "Meta description is set");
            yield return
                metaDescription.Contains(analysisTerm, StringComparison.OrdinalIgnoreCase)
                    ? GetFacet("Meta description contains analysis term", SEOAnalysisStatus.Success, string.Format("Meta description contains '{0}'", analysisTerm))
                    : GetFacet("Meta description contains analysis term", SEOAnalysisStatus.Error, string.Format("Meta description does not contain '{0}'", analysisTerm));
            if (metaDescription.Length < 120)
            {
                yield return
                    GetFacet("Meta description length", SEOAnalysisStatus.CanBeImproved,
                        "Meta description should be at least 120 characters");
            }
            else if (metaDescription.Length > 200)
            {
                yield return
                    GetFacet("Meta description length", SEOAnalysisStatus.Error,
                        "Meta description should be at most 200 characters");
            }
            else
            {
                yield return
                    GetFacet("Meta description length", SEOAnalysisStatus.Success,
                        "Meta description is of optimal length (between 120 and 200 characters)");
            }
            bool anyWithSameDescription =
                _session.QueryOver<Webpage>()
                    .Where(page => page.Site.Id == webpage.Site.Id && page.Id != webpage.Id && page.MetaDescription == webpage.MetaDescription)
                    .Any();

            if (anyWithSameDescription)
                yield return
                    GetFacet("Any other pages with same meta description?", SEOAnalysisStatus.Error, "The meta description is not unique");
            else
                yield return
                    GetFacet("Any other pages with same meta description?", SEOAnalysisStatus.Success, "The meta description is unique");
        }
    }
}