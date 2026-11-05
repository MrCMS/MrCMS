using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.ContentTemplates.ContentTemplateTokenProviders.Base;
using MrCMS.ContentTemplates.Services;
using MrCMS.Website;

namespace MrCMS.ContentTemplates.ContentTemplateTokenProviders;

public class CurrentDateToken(IServiceProvider serviceProvider, IGetDateTimeNow getDateTimeNow)
    : ContentTemplateTokenProvider
{
    public override string Name => "CurrentDate";
    public override string Icon => "fa fa-clock-o";
    public override string HtmlPattern => $"[{Name} name=\"CurrentDate\"]\n[/{Name}]";
    public override string ResponsiveClass => "col-md-6 col-lg-4 col-xl-3";

    public override string Guide =>
        @"<div class='token-guide mt-3'>
        <h6>Available Variables:</h6>
        <div class='mb-3'>
            <strong>CurrentDate Variables:</strong><br>
            <code>CurrentDate.ShortDate</code>, <code>CurrentDate.LongDate</code>, <code>CurrentDate.Year</code>, <code>CurrentDate.Month</code>, <code>CurrentDate.Day</code>
        </div>
        <small class='text-muted'>Use these variables in your template with double curly braces, e.g., <code>{{CurrentDate.ShortDate}}</code></small>
        <small class='text-muted d-block mt-2'>Note: Replace <code>'CurrentDate'</code> with the value of the 'name' attribute in your token.</small>
    </div>";


    public override async Task<string> RenderAsync(
        string innerContent,
        Dictionary<string, string> attributes,
        Dictionary<string, object> variables,
        IHtmlHelper htmlHelper)
    {
        try
        {
            var name = attributes.GetValueOrDefault("name", "CurrentDate");

            var currentDate = getDateTimeNow.LocalNow;

            var tokenVariables = new Dictionary<string, object>(variables)
            {
                { $"{name}.ShortDate", currentDate.ToShortDateString() },
                { $"{name}.LongDate", currentDate.ToLongDateString() },
                { $"{name}.Year", currentDate.ToString("yyyy") },
                { $"{name}.Month", currentDate.ToString("MM") },
                { $"{name}.Day", currentDate.ToString("dd") }
            };

            // Render the inner content with page variables
            var renderer = serviceProvider.GetRequiredService<IContentTemplateRenderer>();
            var renderedContent = await renderer.RenderAsync(htmlHelper, innerContent, tokenVariables)
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