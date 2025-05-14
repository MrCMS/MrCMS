using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.ContentTemplates.ContentTemplateTokenProviders.Base;
using MrCMS.ContentTemplates.Services;
using MrCMS.Helpers;

namespace MrCMS.ContentTemplates.ContentTemplateTokenProviders;

public class DateFormatterTokenProvider(IServiceProvider serviceProvider) : ContentTemplateTokenProvider
{
    public override string Name => "DateFormatter";
    public override string Icon => "fa fa-calendar";
    public override string HtmlPattern => $"[{Name} name=\"DateFormatter\" ]\n[/{Name}] ";

    public override string Guide =>
        @"<div class='token-guide mt-3'>
        <h6>Available Variables:</h6>
        <div class='mb-3'>
            <strong>Date Variables:</strong><br>
            <code>DateFormatter.Date</code> -  formatted date/time<br>
        </div>
        <div class='mb-3'>
            <strong>Attributes:</strong><br>
            <code>format</code> - Required. Date format string (e.g., 'yyyy-MM-dd', 'dd/MM/yyyy', 'MMMM d, yyyy', etc.)<br>
            <code>culture</code> - Optional. Culture code for formatting (e.g., 'en-US', 'fr-FR', etc.)
        </div>
        <div class='mb-3'>
            <strong>Format Examples:</strong><br>
            <code>yyyy-MM-dd</code> - 2023-12-31<br>
            <code>dd/MM/yyyy</code> - 31/12/2023<br>
            <code>MMMM d, yyyy</code> - December 31, 2023<br>
            <code>dddd, MMMM d, yyyy</code> - Sunday, December 31, 2023<br>
            <code>yyyy-MM-dd HH:mm:ss</code> - 2023-12-31 14:30:00<br>
            <code>h:mm tt</code> - 2:30 PM
        </div>
        <small class='text-muted'>Use these variables in your template with double curly braces, e.g., <code>{{DateFormatter.Date}}</code></small>
        <small class='text-muted d-block mt-2'>Note: Replace <code>'DateFormatter'</code> with the value of the 'name' attribute in your token.</small>
    </div>";

    public override async Task<string> RenderAsync(
        string innerContent,
        Dictionary<string, string> attributes,
        Dictionary<string, object> variables,
        IHtmlHelper htmlHelper)
    {
        var name = attributes.GetValueOrDefault("name", "DateFormatter");
        var fieldName = GetFieldName(name);

        // Get the date format from attributes or use default
        variables.TryGetValue($"{fieldName}.Format", out var formatValue);
        var format = formatValue?.ToString() ?? "yyyy-MM-dd";

        // Get the culture from attributes or use current culture
        variables.TryGetValue($"{fieldName}.Culture", out var cultureValue);
        var culture = string.IsNullOrWhiteSpace(cultureValue?.ToString())
            ? CultureInfo.CurrentCulture
            : CultureInfo.GetCultureInfo(cultureValue?.ToString() ?? string.Empty);

        DateTime? dateToFormat = null;

        var renderer = serviceProvider.GetRequiredService<IContentTemplateRenderer>();


        if (attributes.TryGetValue("date", out var customDate))
        {
            if (!string.IsNullOrWhiteSpace(customDate))
            {
                var parsedCustomDate = await renderer.RenderAsync(htmlHelper, customDate, variables);
                if (DateTime.TryParse(parsedCustomDate, out var result))
                {
                    dateToFormat = result;
                }
            }
        }

        // Format the dates based on the specified format
        var formattedDate = !string.IsNullOrWhiteSpace(format)
            ? dateToFormat!?.ToString(format, culture)
            : dateToFormat?.ToString(culture);


        // Create variables dictionary with date values
        var tokenVariables = new Dictionary<string, object>(variables)
        {
            { $"{name}.Date", formattedDate },
        };

        return await renderer.RenderAsync(htmlHelper, innerContent, tokenVariables);
    }

    public override async Task<string> RenderAdminAsync(
        string innerContent,
        Dictionary<string, string> attributes,
        IHtmlHelper htmlHelper,
        Dictionary<string, object> savedProperties = null)
    {
        var name = attributes.GetValueOrDefault("name", "DateFormatter");
        var fieldId = GetFieldId(name);
        var fieldName = GetFieldName(name);

        // Get format from attributes or use default
        var format = savedProperties?.GetValueOrDefault($"{fieldName}.Format")?.ToString() ?? "yyyy-MM-dd";

        // Get culture from attributes or use default
        var culture = savedProperties?.GetValueOrDefault($"{fieldName}.Culture")?.ToString() ?? string.Empty;

        var formatHtml = $@"
            <div class='form-group'>
                <label for='{fieldId}_format'>Format</label>
                <input type='text' 
                       class='form-control' 
                       id='{fieldId}_format' 
                        name='{fieldName}.Format'
                       value='{format}'
                       />
                <small class='form-text text-muted'>Format string for date formatting</small>
            </div>";

        var cultureHtml = $@"
            <div class='form-group'>
                <label for='{fieldId}_culture'>Culture</label>
                <input type='text' 
                       class='form-control' 
                       id='{fieldId}_culture' 
                        name='{fieldName}.Culture'
                       value='{culture}'
                        />
                <small class='form-text text-muted'>Culture code for date formatting (e.g., en-US, fr-FR)</small>
            </div>";

        var exampleHtml = $@"
            <div class='form-group'>
                <label>Example Output</label>
                <div class='form-control-plaintext'>
                    {DateTime.Now.ToString(format, string.IsNullOrWhiteSpace(culture) ? CultureInfo.CurrentCulture : CultureInfo.GetCultureInfo(culture))}
                </div>
                <small class='form-text text-muted'>Current date/time with the specified format</small>
            </div>";

        var renderer = serviceProvider.GetRequiredService<IContentTemplateRenderer>();
        var innerContentHtml = await RenderInnerContentAsync(
                htmlHelper, innerContent, savedProperties, renderer)
            .ConfigureAwait(false);

        var html = $@"
            <div class='row'>
                <div class='col-md-6'>
                    {formatHtml}
                </div>
                <div class='col-md-6'>
                    {cultureHtml}
                </div>
                <div class='col-md-12'>
                    {exampleHtml}
                </div>
            </div>";

        if (!string.IsNullOrWhiteSpace(innerContentHtml))
        {
            html += innerContentHtml;
        }

        return BuildCardHtml(name.BreakUpString(), html);
    }

    private DateTime? TryGetDateAttribute(Dictionary<string, object> attributes, string key, DateTime defaultValue)
    {
        return attributes.TryGetValue(key, out var value) && DateTime.TryParse(value?.ToString(), out var result)
            ? result
            : defaultValue;
    }
}