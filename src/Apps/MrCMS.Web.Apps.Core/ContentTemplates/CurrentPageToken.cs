using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.ContentTemplates.ContentTemplateTokenProviders.Base;
using MrCMS.ContentTemplates.Services;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Core.ContentTemplates;

public class CurrentPageToken(IServiceProvider serviceProvider, IGetCurrentPage getCurrentPage)
    : ContentTemplateTokenProvider<MrCMSCoreApp>
{
    public override string Name => "CurrentPage";
    public override string Icon => "fa fa-file-text-o";
    public override string HtmlPattern => $"[{Name} name=\"CurrentPage\"]\n[/{Name}]";
    public override string ResponsiveClass => "col-md-6 col-lg-4 col-xl-3";

    public override string Guide =>
        @"<div class='token-guide mt-3'>
        <h6>Available Variables:</h6>
        <div class='mb-3'>
            <strong>CurrentPage Variables:</strong><br>
            <code>CurrentPage.Id</code>, <code>CurrentPage.Name</code>, <code>CurrentPage.FeatureImage</code>, <code>CurrentPage.PublishOn</code>, <code>CurrentPage.Published</code>, <code>CurrentPage.Url</code>, <code>CurrentPage.Tags</code>
        </div>
        <small class='text-muted'>Use these variables in your template with double curly braces, e.g., <code>{{CurrentPage.Name}}</code></small>
        <small class='text-muted d-block mt-2'>Note: Replace <code>'CurrentPage'</code> with the value of the 'name' attribute in your token.</small>
    </div>";


    public override async Task<string> RenderAsync(
        string innerContent,
        Dictionary<string, string> attributes,
        Dictionary<string, object> variables,
        IHtmlHelper htmlHelper)
    {
        try
        {
            var name = attributes.GetValueOrDefault("name", "Page");
            
            var page = getCurrentPage.GetPage();

            if (page == null)
                return string.Empty;
            
            var pageVariables = new Dictionary<string, object>(variables)
            {
                { $"{name}.Id", page.Id },
                { $"{name}.Name", page.Name },
                { $"{name}.FeatureImage", page is TextPage textPage ? textPage.FeatureImage : "" },
                { $"{name}.PublishOn", page.PublishOn },
                { $"{name}.Published", page.Published },
                { $"{name}.Url", $"/{page.UrlSegment}" },
                { $"{name}.Tags", string.Join(", ", page.Tags.Select(t => t.Name)) },
            };

            // Render the inner content with page variables
            var renderer = serviceProvider.GetRequiredService<IContentTemplateRenderer>();
            var renderedContent = await renderer.RenderAsync(htmlHelper, innerContent, pageVariables)
                .ConfigureAwait(false);
            
            return renderedContent;
        }
        catch
        {
            // For now, return an empty string
            return string.Empty;
        }
    }

    public override async Task<string> RenderAdminAsync(
        string innerContent,
        Dictionary<string, string> attributes,
        IHtmlHelper htmlHelper,
        Dictionary<string, object> savedProperties = null)
    {
        
        var renderer = serviceProvider.GetRequiredService<IContentTemplateRenderer>();
        var innerContentHtml = await RenderInnerContentAsync(
                htmlHelper, innerContent, savedProperties, renderer)
            .ConfigureAwait(false);

        return innerContentHtml;
    }
}