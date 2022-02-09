using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Models;

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

        public void SetNoCanonical()
        {
            this[LinkTag.Canonical] = string.Empty;
        }

        private IHtmlContent RenderPair(KeyValuePair<string, string> tag)
        {
            if (string.IsNullOrWhiteSpace(tag.Key) || string.IsNullOrWhiteSpace(tag.Value))
                return HtmlString.Empty;
                
            var tagBuilder = new TagBuilder("link");
            tagBuilder.Attributes["rel"] = tag.Key;
            tagBuilder.Attributes["href"] = tag.Value;
            return tagBuilder;
        }
    }
}