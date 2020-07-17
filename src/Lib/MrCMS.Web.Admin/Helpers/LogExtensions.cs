using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MrCMS.Web.Admin.Helpers
{
    public static class LogExtensions
    {
        public static ErrorContextData GetContextData(this Log log)
        {

            try
            {
                return JsonConvert.DeserializeObject<ErrorContextData>(log?.RequestData);
            }
            catch 
            {
                return null;
            }
        }

        public static bool CanParseExceptionData(this Log log)
        {
            try
            {
                JToken.Parse(log.ExceptionData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IDictionary<string, string> ExceptionDataDictionary(this Log log)
        {
            var dictionary = new Dictionary<string, string>();
            if (!CanParseExceptionData(log))
                return dictionary;

            var jToken = JToken.Parse(log.ExceptionData);
            foreach (var child in jToken.Children<JProperty>())
            {
                var value = child.Value;
                var data = value?.ToString();
                if (!string.IsNullOrWhiteSpace(data))
                {
                    dictionary[child.Path] = data;
                }
            }

            return dictionary;
        }
        public static IHtmlContent FormatExceptionData(this Log log)
        {
            if (string.IsNullOrWhiteSpace(log?.ExceptionData))
                return new HtmlString("-");

            try
            {
                var jToken = JToken.Parse(log.ExceptionData);
                var content = jToken.ToString(Formatting.Indented);
                content = content.Replace("\\r\\n", Environment.NewLine);
                var preTag = new TagBuilder("pre");
                preTag.InnerHtml.Append(content);
                return preTag;
            }
            catch
            {
                return new HtmlString("-");
            }
        }
    }
}