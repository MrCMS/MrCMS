using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Helpers.TagHelpers
{
    [HtmlTargetElement("toggle-button")]
    public class SwitchTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();

            var divSlider = new TagBuilder("div");
            divSlider.AddCssClass("slider round");

         
            output.TagName = "label";
            output.Attributes.Add("class", "switch");
            output.Content.AppendHtml(childContent);
            output.Content.AppendHtml(divSlider);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }

    //public class AdminHeaderTitleTagHelper : TagHelper
    //{
    //    public string Title { get; set; }
    //    public override void Process(TagHelperContext context, TagHelperOutput output)
    //    {
    //        output.TagName = "div";
    //        output.AddClass("col-lg-6", HtmlEncoder.Default);
    //        output.AddClass("col-xl-7", HtmlEncoder.Default);
    //        output.Content.SetHtmlContent($@"<h1 class=""text-truncate"">{Title}</h1>");
    //        output.TagMode = TagMode.StartTagAndEndTag;
    //    }
    //}
}
