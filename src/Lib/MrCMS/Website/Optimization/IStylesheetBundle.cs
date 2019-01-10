using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MrCMS.Website.Optimization
{
    public interface IStylesheetBundle
    {
        string Url { get; }
        IEnumerable<string> Files { get; }
    }
}