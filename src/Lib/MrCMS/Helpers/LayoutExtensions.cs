using System.Text.RegularExpressions;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Helpers
{
    public static class LayoutExtensions
    {
        public static string GetLayoutName(this Layout layout)
        {
            return !string.IsNullOrWhiteSpace(layout.UrlSegment)
                ? layout.UrlSegment
                : string.Format("_{0}", Regex.Replace(layout.Name, @"[^\w//]+", ""));
        }
    }
}