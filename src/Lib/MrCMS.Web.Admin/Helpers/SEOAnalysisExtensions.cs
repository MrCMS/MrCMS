using System;
using MrCMS.Web.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Admin.Helpers
{
    public static class SEOAnalysisExtensions
    {
        public static string GetTableClass(this SEOAnalysisStatus status)
        {
            switch (status)
            {
                case SEOAnalysisStatus.Success:
                    return "table-success";
                case SEOAnalysisStatus.CanBeImproved:
                    return "table-warning";
                case SEOAnalysisStatus.Error:
                    return "table-danger";
                default:
                    throw new ArgumentOutOfRangeException("status");
            }
        }

        public static string GetIconClass(this SEOAnalysisStatus status)
        {

            switch (status)
            {
                case SEOAnalysisStatus.CanBeImproved:
                    return "fa-arrow-right";
                case SEOAnalysisStatus.Error:
                    return "fa-remove";
                case SEOAnalysisStatus.Success:
                    return "fa-check";
                default:
                    throw new ArgumentOutOfRangeException("status");
            }
        }
    }
}