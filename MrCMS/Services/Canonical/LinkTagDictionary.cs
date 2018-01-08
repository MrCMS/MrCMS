using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MrCMS.Services.Canonical
{
    public class LinkTagDictionary : Dictionary<string, string>
    {
        public LinkTagDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {

        }

        public MvcHtmlString Render()
        {
            return MvcHtmlString.Create(string.Join("", this.Select(RenderPair)));
        }

        private string RenderPair(KeyValuePair<string, string> tag)
        {
            return string.Format(@"<link href=""{0}"" rel=""{1}"" />", tag.Value, tag.Key);
        }
    }
}