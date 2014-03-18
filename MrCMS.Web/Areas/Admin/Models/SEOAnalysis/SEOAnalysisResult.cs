using System.Collections.Generic;
using MrCMS.Web.Areas.Admin.Controllers;

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