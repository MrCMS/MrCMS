using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace MrCMS.Shortcodes.Forms
{
    public static class FormRenderingHelper
    {
        
        public static string GetString(this IHtmlContent content)
        {
            using (var writer = new System.IO.StringWriter())
            {        
                content.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            } 
        }
    }
}