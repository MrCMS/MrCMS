using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MrCMS.Web.Apps.Admin.Infrastructure.TagHelpers
{
    public class AdminHeaderTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "section";
            output.AddClass("content-header", HtmlEncoder.Default);
            output.AddClass("container-fluid", HtmlEncoder.Default);
            output.AddClass("mb-0", HtmlEncoder.Default);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }

    public class RowTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.AddClass("row", HtmlEncoder.Default);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }

    public class AdminHeaderTitleTagHelper : TagHelper
    {
        public string Title { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.AddClass("col-lg-6", HtmlEncoder.Default);
            output.AddClass("col-xl-7", HtmlEncoder.Default);
            output.Content.SetHtmlContent($@"<h1 class=""text-truncate"">{Title}</h1>");
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }

    public class AdminHeaderActionsTagHelper : TagHelper
    {
        public string Title { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.AddClass("col-lg-6", HtmlEncoder.Default);
            output.AddClass("col-xl-5", HtmlEncoder.Default);
            output.AddClass("text-right", HtmlEncoder.Default);
            
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }

    public class CardTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.AddClass("card", HtmlEncoder.Default);
        }
    }

    public class CardBodyTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.AddClass("card-body", HtmlEncoder.Default);
        }
    }
}