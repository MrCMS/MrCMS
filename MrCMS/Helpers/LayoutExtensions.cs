using System.Collections.Generic;
using System.Text.RegularExpressions;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Helpers
{
    public static class LayoutExtensions
    {
        public static IEnumerable<LayoutArea> GetLayoutAreas(this Layout layout)
        {
            Layout current = layout;
            var layoutAreas = new List<LayoutArea>();
            while (current != null)
            {
                layoutAreas.AddRange(current.LayoutAreas);
                current = current.Parent.Unproxy() as Layout;
            }

            return layoutAreas;
        }

        public static string GetLayoutName(this Layout layout)
        {
            return !string.IsNullOrWhiteSpace(layout.UrlSegment)
                ? layout.UrlSegment
                : string.Format("_{0}", Regex.Replace(layout.Name, @"[^\w//]+", ""));
        }
    }
}