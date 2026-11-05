using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.ContentTemplates.ContentTemplateTokenProviders.Base;
using MrCMS.ContentTemplates.Services;
using MrCMS.Helpers;

namespace MrCMS.ContentTemplates.ContentTemplateTokenProviders;

public class MediaUrlTokenProvider(IServiceProvider serviceProvider) : ContentTemplateTokenProvider
{
    public override string Name => "MediaUrl";
    public override string Icon => "fa fa-picture-o";
    public override string HtmlPattern => $"[{Name} name=\"MediaUrl\" width=\"\" height=\"\"][/{Name}]";
    
    public override string Guide =>
        @"<div class='token-guide mt-3'>
        <h6>Available Variables:</h6>
        <div class='mb-3'>
            <strong>Media Variables:</strong><br>
            <code>MediaUrl.Url</code>
        </div>
        <small class='text-muted'>Use these variables in your template with double curly braces, e.g., <code>{{MediaUrl.Url}}</code></small>
        <small class='text-muted d-block mt-2'>Note: Replace <code>'MediaUrl'</code> with the value of the 'name' attribute in your token.</small>
    </div>";

    public override async Task<string> RenderAsync(
        string innerContent,
        Dictionary<string, string> attributes,
        Dictionary<string, object> variables,
        IHtmlHelper htmlHelper)
    {
        if (!attributes.TryGetValue("name", out var name))
            return string.Empty;
        var fieldName = GetFieldName(name);
        
        string mediaUrl;
        if (variables.TryGetValue(fieldName, out var mediaUrlValue) && mediaUrlValue is string url && !string.IsNullOrWhiteSpace(url))
            mediaUrl = url;
        else
            return string.Empty;


        attributes.TryGetValue("width", out var widthValue);
        attributes.TryGetValue("height", out var heightValue);
        
        var width = TryGetIntVariable(widthValue);
        var height = TryGetIntVariable(heightValue);
        
        var size = default(Size);
        if (width is > 0)
            size = new Size { Width = width.Value };

        if (height is > 0)
            size.Height = height.Value;

    
        var renderedMediaUrl = await htmlHelper.GetImageUrl(mediaUrl, size);
        
        var tokenVariables = new Dictionary<string, object>(variables)
        {
            { $"{name}.Url", renderedMediaUrl }
        };
        
        // Render the inner content with the key variable
        var renderer = serviceProvider.GetRequiredService<IContentTemplateRenderer>();
        return await renderer.RenderAsync(htmlHelper, innerContent, tokenVariables);
    }

    public override Task<string> RenderAdminAsync(
        string innerContent,
        Dictionary<string, string> attributes,
        IHtmlHelper htmlHelper,
        Dictionary<string, object> savedProperties = null)
    {
        if (!attributes.TryGetValue("name", out var name))
            return Task.FromResult(string.Empty);
        var fieldId = GetFieldId(name);
        var fieldName = GetFieldName(name);
        
        var value = string.Empty;

        // Use saved data if available
        if (savedProperties?.TryGetValue(fieldName, out var savedValue) == true)
        {
            value = savedValue?.ToString() ?? string.Empty;
        }

        return Task.FromResult($@"
        <div class='form-group'>
            <label class='form-label d-block' for='{fieldId}'>{name.BreakUpString()}</label>
            <input type='text' 
                   class='form-control' 
                   id='{fieldId}' 
                   name='{fieldName}' 
                   value='{HttpUtility.HtmlEncode(value)}'
                   data-type='media-selector'/>
        </div>");
    }
    
    private int? TryGetIntVariable(string value)
    {
        return int.TryParse(value, out var result)
            ? result
            : null;
    }
}