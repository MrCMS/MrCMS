using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;

namespace MrCMS.Web.Apps.Core.Models.Navigation
{
    public class NavigationRecord
    {
        public NavigationRecord(string text, string url, [CanBeNull] Type type = null,
            IEnumerable<NavigationRecord> children = null)
        {
            Text = new HtmlString(text);
            Url = new HtmlString(url);
            Type = type;
            Children = children?.ToList() ?? new List<NavigationRecord>();
        }

        public IHtmlContent Text { get; }

        public IHtmlContent Url { get; }
        public Type Type { get; }

        public List<NavigationRecord> Children { get; }
    }
}