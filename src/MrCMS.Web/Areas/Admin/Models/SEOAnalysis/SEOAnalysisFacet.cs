using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Models.SEOAnalysis
{
    public class SEOAnalysisFacet
    {
        public string Name { get; set; }
        public SEOAnalysisStatus Status { get; set; }
        public List<string> Messages { get; set; }
        public int Importance { get; set; }
    }
}