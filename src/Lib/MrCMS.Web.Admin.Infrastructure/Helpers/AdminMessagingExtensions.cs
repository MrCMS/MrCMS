using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MrCMS.Web.Admin.Infrastructure.Helpers
{
    public static class AdminMessagingExtensions
    {
        private const string SuccessMessageKey = "success-message";
        private const string ErrorMessageKey = "error-message";
        private const string InfoMessageKey = "info-message";

        public static void AddSuccessMessage(this ITempDataDictionary tempDataDictionary, string message)
        {
            var messages = new List<string>(tempDataDictionary.SuccessMessages()) {message};
            tempDataDictionary[SuccessMessageKey] = messages;
        }
        
        public static IReadOnlyCollection<string> SuccessMessages(this ITempDataDictionary tempData)
        {
            if (tempData.ContainsKey(SuccessMessageKey))
                return tempData[SuccessMessageKey] as IReadOnlyCollection<string>;
            return Array.Empty<string>();
        }
        
        public static void AddErrorMessage(this ITempDataDictionary tempDataDictionary, string message)
        {
            var messages = new List<string>(tempDataDictionary.ErrorMessages()) {message};
            tempDataDictionary[ErrorMessageKey] = messages;
        }

        public static IReadOnlyCollection<string> ErrorMessages(this ITempDataDictionary tempData)
        {
            if (tempData.ContainsKey(ErrorMessageKey))
                return tempData[ErrorMessageKey] as IReadOnlyCollection<string>;
            return Array.Empty<string>();
        }
        
        public static void AddInfoMessage(this ITempDataDictionary tempDataDictionary, string message)
        {
            var messages = new List<string>(tempDataDictionary.InfoMessages()) {message};
            tempDataDictionary[InfoMessageKey] = messages;
        }

        public static IReadOnlyCollection<string> InfoMessages(this ITempDataDictionary tempData)
        {
            if (tempData.ContainsKey(InfoMessageKey))
                return tempData[InfoMessageKey] as IReadOnlyCollection<string>;
            return Array.Empty<string>();
        }
    }
}