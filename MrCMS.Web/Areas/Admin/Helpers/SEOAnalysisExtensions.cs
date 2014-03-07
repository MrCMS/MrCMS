using System;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class SEOAnalysisExtensions
    {
        public static string GetTableClass(this SEOAnalysisStatus status)
        {
            switch (status)
            {
                case SEOAnalysisStatus.Success:
                    return "success";
                case SEOAnalysisStatus.CanBeImproved:
                    return "can-be-improved";
                case SEOAnalysisStatus.Problem:
                    return "problem";
                case SEOAnalysisStatus.Error:
                    return "error";
                default:
                    throw new ArgumentOutOfRangeException("status");
            }
        }

        public static string GetIconClass(this SEOAnalysisStatus status)
        {

            switch (status)
            {
                case SEOAnalysisStatus.CanBeImproved:
                    return "icon-arrow-right";
                case SEOAnalysisStatus.Error:
                    return "icon-remove";
                case SEOAnalysisStatus.Success:
                    return "icon-ok";
                case SEOAnalysisStatus.Problem:
                    return "icon-remove";
                default:
                    throw new ArgumentOutOfRangeException("status");
            }
        }
    }
}