using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.ContentTemplates.ContentTemplateTokenProviders.Base;
using MrCMS.ContentTemplates.Services;
using MrCMS.Services;

namespace MrCMS.ContentTemplates.ContentTemplateTokenProviders;

public class LoggedInUserTokenProvider(IServiceProvider serviceProvider) : ContentTemplateTokenProvider
{
    public override string Name => "LoggedInUser";
    public override string Icon => "fa fa-user";
    public override string HtmlPattern => $"[{Name} name=\"User\"]\n[/{Name}]";
    
    public override string Guide =>
        @"<div class='token-guide mt-3'>
        <h6>Available Variables:</h6>
        <div class='mb-3'>
            <strong>LoggedInUser Variables:</strong><br>
            <code>User.IsAuthenticated</code>, <code>User.Id</code>, <code>User.Email</code>, <code>User.FirstName</code>, <code>User.LastName</code>, <code>User.FullName</code>, <code>User.AvatarImage</code>, <code>User.PhoneNumber</code>, <code>User.IsAdmin</code>
        </div>
        <small class='text-muted'>Use these variables in your template with double curly braces, e.g., <code>{{User.FullName}}</code></small>
        <small class='text-muted d-block mt-2'>Note: Replace <code>'User'</code> with the value of the 'name' attribute in your token.</small>
    </div>";

    
    public override async Task<string> RenderAsync(
        string innerContent,
        Dictionary<string, string> attributes,
        Dictionary<string, object> variables,
        IHtmlHelper htmlHelper)
    {
        if (!attributes.TryGetValue("name", out var name))
            return string.Empty;
        
        var getCurrentUser = serviceProvider.GetRequiredService<IGetCurrentUser>();
        
        if (getCurrentUser == null)
            return string.Empty;

        var user = await getCurrentUser.Get();

        var tokenVariables = new Dictionary<string, object>(variables)
        {
            { $"{name}.IsAuthenticated", user != null },
            { $"{name}.Id", user?.Id },
            { $"{name}.Email", user?.Email },
            { $"{name}.FirstName", user?.FirstName },
            { $"{name}.LastName", user?.LastName },
            { $"{name}.FullName", user?.Name },
            { $"{name}.AvatarImage", user?.AvatarImage },
            { $"{name}.PhoneNumber", user?.PhoneNumber },
            { $"{name}.IsAdmin", user?.IsAdmin }
        };

        var renderer = serviceProvider.GetRequiredService<IContentTemplateRenderer>();
        return await renderer.RenderAsync(htmlHelper, innerContent, tokenVariables);
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