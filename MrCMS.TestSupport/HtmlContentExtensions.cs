using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace MrCMS.TestSupport
{
    public static class HtmlContentExtensions
    {
        public static string AsAString(this IHtmlContent htmlContent)
        {
            var writer = new StringWriter();
            htmlContent.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}