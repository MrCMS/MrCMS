using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Models.SEOAnalysis
{
    public class SEOAnalysisResult : List<SEOAnalysisFacet>
    {
        public SEOAnalysisResult() { }
        public SEOAnalysisResult(IEnumerable<SEOAnalysisFacet> facets)
            : base(facets)
        {

        }
    }
}