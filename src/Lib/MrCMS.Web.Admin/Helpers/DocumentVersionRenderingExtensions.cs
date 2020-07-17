using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace MrCMS.Web.Admin.Helpers
{
    public static class DocumentVersionRenderingExtensions
    {
        public static IHtmlContent RenderValue(this IHtmlHelper htmlHelper, object value)
        {
            return value != null
                ? new HtmlString(IsJson(value.ToString())
                    ? GetPrettyPrintedJson(value.ToString())
                    : value.ToString())
                : HtmlString.Empty;
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