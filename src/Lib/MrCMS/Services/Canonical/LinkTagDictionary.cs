using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Services.Canonical
{
    public class LinkTagDictionary : Dictionary<string, string>
    {
        public LinkTagDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public IHtmlContent Render()
        {
            IHtmlContentBuilder builder = new HtmlContentBuilder();

            foreach (var pair in this)
                builder = builder.AppendHtml(RenderPair(pair));

            return builder;
        }

        private IHtmlContent RenderPair(KeyValuePair<string, string> tag)
        {
            var tagBuilder = new TagBuilder("link");
            tagBuilder.Attributes["rel"] = tag.Key;
            tagBuilder.Attributes["href"] = tag.Value;
            return tagBuilder;
        }
    }
}