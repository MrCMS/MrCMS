using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MrCMS.Web.Admin.Infrastructure.Helpers
{
    public static class AdminMessagingExtensions
    {
        public static ICollection<string> SuccessMessages(this ITempDataDictionary tempData)
        {
            if (!tempData.ContainsKey("success-message")) tempData["success-message"] = new List<string>();
            return tempData["success-message"] as ICollection<string>;
        }

        public static ICollection<string> ErrorMessages(this ITempDataDictionary tempData)
        {
            if (!tempData.ContainsKey("error-message")) tempData["error-message"] = new List<string>();
            return tempData["error-message"] as ICollection<string>;
        }

        public static ICollection<string> InfoMessages(this ITempDataDictionary tempData)
        {
            if (!tempData.ContainsKey("info-message")) tempData["info-message"] = new List<string>();
            return tempData["info-message"] as ICollection<string>;
        }
    }
}