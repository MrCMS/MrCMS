using System.Collections.Generic;

namespace MrCMS.Web.Admin.Models.SEOAnalysis
{
    public class SEOAnalysisResult : List<SEOAnalysisFacet>
    {
        public SEOAnalysisResult() { }
        public SEOAnalysisResult(IReadOnlyList<SEOAnalysisFacet> facets)
            : base(facets)
        {

        }
    }
}