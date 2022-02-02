using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Entities.BlockItems;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

[Display(Name = "Quote")]
public class Quote : IContentBlock
{
    public Quote()
    {
        CssClasses = "border-left border-secondary pl-2";
    }
    public string QuoteText { get; set; }
    public string QuoteFooter { get; set; }
    public string CssClasses { get; set; }
    public IReadOnlyList<BlockItem> Items => new BlockItem[] { };
}