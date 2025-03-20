using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.ContentTemplates.ContentTemplateTokenProviders.Base;
using MrCMS.ContentTemplates.Services;
using NCalc;

namespace MrCMS.ContentTemplates.ContentTemplateTokenProviders;

public class ConditionTokenProvider(IServiceProvider serviceProvider) : ContentTemplateTokenProvider
{
    public override string Name => "Condition";
    public override string Icon => "fa fa-terminal";
    public override string HtmlPattern => $"[{Name} if=\"\"][/{Name}]";
    
    public override string Guide =>
        @"<div class='token-guide mt-3'>
        <h6>Available Expressions & Operators:</h6>
        <div class='mb-3'>
            <strong>Logical Operators:</strong> <code>&amp;&amp;</code> (AND), <code>||</code> (OR), <code>!</code> (NOT) <br>
            <strong>Comparison Operators:</strong> <code>==</code>, <code>!=</code>, <code>&gt;</code>, <code>&lt;</code>, <code>&gt;=</code>, <code>&lt;=</code> <br>
            <strong>Mathematical Operators:</strong> <code>+</code>, <code>-</code>, <code>*</code>, <code>/</code>, <code>%</code> <br>
            <strong>Parentheses:</strong> Use <code>( )</code> for grouping expressions.
        </div>

        <h6>Custom Functions:</h6>
        <div class='mb-3'>
            <strong>Len(value)</strong> → Returns the length of the given string. <br>
            <small>Example: <code>Len(User.FirstName) &gt; 0</code></small> <br>
            <strong>IsEmpty(value)</strong> → Returns <code>true</code> if the string is null or empty. <br>
            <small>Example: <code>IsEmpty(User.Email)</code></small>
        </div>

        <h6>Examples:</h6>
        <div class='mb-3'>
            <code>User.IsAuthenticated &amp;&amp; !IsEmpty(User.Email)</code> → Checks if user is logged in and has an email. <br>
            <code>Len(User.FirstName) &gt; 3</code> → Checks if first name has more than 3 characters. <br>
            <code>User.IsAdmin || User.Email == 'admin@example.com'</code> → Checks if user is an admin or has a specific email.
        </div>

        <small class='text-muted'>Use these expressions in your condition tokens.</small>
    </div>";


    public override async Task<string> RenderAsync(
        string innerContent,
        Dictionary<string, string> attributes,
        Dictionary<string, object> variables,
        IHtmlHelper htmlHelper)
    {
        if (!attributes.TryGetValue("if", out var condition))
            return string.Empty;

        try
        {
            // Create a new dictionary to hold the modified variables
            var modifiedVariables = new Dictionary<string, object>();

            // Iterate over the original variables
            foreach (var kvp in variables)
            {
                if (!condition.Contains(kvp.Key))
                {
                    // If the variable is not used in the condition, skip it
                    continue;
                }
                
                // Replace dots in the variable name with underscores
                var modifiedKey = kvp.Key.Replace('.', '_');
                modifiedVariables[modifiedKey] = kvp.Value;

                // Replace occurrences of the original variable name in the condition
                condition = condition.Replace(kvp.Key, modifiedKey);
            }

            // Create the expression with the modified condition and variables
            var exp = new Expression(condition)
            {
                Parameters = modifiedVariables
            };
            
            
            //Custom functions
            exp.EvaluateFunction += (name, args) =>
            {
                if (name == "Len")
                {
                    args.Result = args.Parameters[0].Evaluate()?.ToString()?.Length ?? 0;
                }
                else if (name == "IsEmpty")
                {
                    var value = args.Parameters[0].Evaluate()?.ToString();
                    args.Result = string.IsNullOrWhiteSpace(value);
                }
            };

            if (exp.Evaluate() is true)
            {
                var renderer = serviceProvider.GetRequiredService<IContentTemplateRenderer>();
                return await renderer.RenderAsync(htmlHelper, innerContent, variables);
            }
        }
        catch
        {
            // If expression evaluation fails, don't render the content
        }

        return string.Empty;
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