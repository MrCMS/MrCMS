using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    // Updated pattern to show else is available
    public override string HtmlPattern => $"[{Name} if=\"\"]\n  // Content if condition is true\n[Else]\n  // Content if condition is false\n[/{Name}]";
    
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

        <h6>Using Else:</h6>
        <div class='mb-3'>
            <p>You can include an <code>[Else]</code> tag to provide alternative content when the condition is false:</p>
            <code>[Condition if='User.IsAuthenticated']</code>
          Welcome back, <code>{{User.FullName}}</code>!
        <code>[Else]</code>
          Please log in to continue.
        <code>[/Condition]</code>
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
            // We'll let the main renderer handle nested tokens
            var renderer = serviceProvider.GetRequiredService<IContentTemplateRenderer>();
            
            // Split content by [Else] tag, respecting nested conditions
            var contentParts = SplitByElseTag(innerContent);
            var ifContent = contentParts.Item1;
            var elseContent = contentParts.Item2;

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
            
            // Evaluate condition and render appropriate content
            if (exp.Evaluate() is true)
            {
                return await renderer.RenderAsync(htmlHelper, ifContent, variables);
            }

            if (!string.IsNullOrEmpty(elseContent))
            {
                return await renderer.RenderAsync(htmlHelper, elseContent, variables);
            }
        }
        catch (Exception ex)
        {
            // Optional: Log the error
            // _logger.LogError(ex, "Error evaluating condition: {Condition}", condition);
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
    
    private (string, string) SplitByElseTag(string content)
{
    // We need to track opening and closing condition tags to match the correct Else
    var tokenRegex = new Regex(@"\[(?<closing>/)?(?<name>\w+)(?<attributes>(?:\s+\w+\s*=\s*(?:""[^""]*""|'[^']*'))*)\s*(?<selfClosing>/)?\]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    // Initialize variables to track position and nesting level
    var currentPos = 0;
    var nestingLevel = 0;
    
    // Scan through the content looking for tokens
    while (currentPos < content.Length)
    {
        var match = tokenRegex.Match(content, currentPos);
        if (!match.Success)
            break;
            
        var tokenName = match.Groups["name"].Value;
        var isClosing = match.Groups["closing"].Success;
        var isSelfClosing = match.Groups["selfClosing"].Success;
        
        // Check if we found an Else tag at the top level (nestingLevel == 0)
        // This would be an Else that belongs to the current condition we're processing
        if (tokenName.Equals("Else", StringComparison.OrdinalIgnoreCase) && nestingLevel == 0)
        {
            // Found the correct [Else] tag for this condition
            var ifContent = content.Substring(0, match.Index);
            var elseContent = content.Substring(match.Index + match.Length);
            return (ifContent, elseContent);
        }
        
        // Update nesting level based on Condition tags
        if (tokenName.Equals("Condition", StringComparison.OrdinalIgnoreCase))
        {
            if (!isClosing && !isSelfClosing)
            {
                // Opening a new nested condition
                nestingLevel++;
            }
            else if (isClosing)
            {
                // Closing a nested condition
                nestingLevel--;
            }
        }
        
        // Move to the next position
        currentPos = match.Index + match.Length;
    }
    
    // If no [Else] tag found at the appropriate level, return original content
    return (content, string.Empty);
}
}