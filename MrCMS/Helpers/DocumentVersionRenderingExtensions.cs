using System;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace MrCMS.Helpers
{
    public static class DocumentVersionRenderingExtensions
    {
        public static MvcHtmlString RenderValue(this HtmlHelper htmlHelper, object value)
        {
            return value != null
                       ? MvcHtmlString.Create(IsJson(value.ToString())
                                                  ? GetPrettyPrintedJson(value.ToString())
                                                  : value.ToString())
                       : MvcHtmlString.Create(String.Empty);
        }

        private static string GetPrettyPrintedJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            var prettyPrintedJson = string.Format("<pre>{0}</pre>",
                                                  JsonConvert.SerializeObject(parsedJson, Formatting.Indented));
            return prettyPrintedJson;
        }

        private static bool IsJson(string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }
    }
}