using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class AdminMessagingExtensions
    {
        public static List<string> SuccessMessages(this TempDataDictionary tempData)
        {
            if (!tempData.ContainsKey("success-message"))
            {
                tempData["success-message"] = new List<string>();
            }
            return tempData["success-message"] as List<string>;
        }

        public static List<string> ErrorMessages(this TempDataDictionary tempData)
        {
            if (!tempData.ContainsKey("error-message"))
            {
                tempData["error-message"] = new List<string>();
            }
            return tempData["error-message"] as List<string>;
        }

        public static List<string> InfoMessages(this TempDataDictionary tempData)
        {
            if (!tempData.ContainsKey("info-message"))
            {
                tempData["info-message"] = new List<string>();
            }
            return tempData["info-message"] as List<string>;
        }
    }
}