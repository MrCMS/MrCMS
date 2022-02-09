using MrCMS.Entities.Documents.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

[Display(Name = "Text")]
public class Text : IContentBlock
{
    public string Heading { get; set; }
    public string Subtext { get; set; }

    public IReadOnlyList<BlockItem> Items => new BlockItem[] { };
}