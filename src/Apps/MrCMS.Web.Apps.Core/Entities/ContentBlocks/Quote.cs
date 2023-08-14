using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

public class Quote : IContentBlock
{
    public string DisplayName => "Quote";
    public string QuoteText { get; set; }
    public string QuoteFooter { get; set; }
    public string CssClasses { get; set; } = "border-left border-secondary pl-2";
    public IReadOnlyList<BlockItem> Items => new BlockItem[] { };
}