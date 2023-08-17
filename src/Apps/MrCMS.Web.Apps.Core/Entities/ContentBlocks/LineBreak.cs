using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

public class LineBreak : IContentBlock
{
    public string DisplayName => "Line Break";
    public string CssClasses { get; set; }
    public IReadOnlyList<BlockItem> Items => new BlockItem[] { };
}