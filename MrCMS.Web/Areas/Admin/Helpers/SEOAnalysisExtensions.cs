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
                    return "danger";
                default:
                    throw new ArgumentOutOfRangeException("status");
            }
        }

        public static string GetIconClass(this SEOAnalysisStatus status)
        {

            switch (status)
            {
                case SEOAnalysisStatus.CanBeImproved:
                    return "glyphicon-arrow-right";
                case SEOAnalysisStatus.Error:
                    return "glyphicon-remove";
                case SEOAnalysisStatus.Success:
                    return "glyphicon-ok";
                case SEOAnalysisStatus.Problem:
                    return "glyphicon-remove";
                default:
                    throw new ArgumentOutOfRangeException("status");
            }
        }
    }
}