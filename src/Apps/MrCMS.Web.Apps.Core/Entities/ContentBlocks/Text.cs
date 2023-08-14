using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

public class Text : IContentBlock
{
    public string DisplayName => "Text";
    public string Heading { get; set; }
    public Alignment HeadingAlignment { get; set; } = Alignment.Start;
    public string Subtext { get; set; }

    public IReadOnlyList<BlockItem> Items => new BlockItem[] { };

    public enum Alignment
    {
        Start,
        Center,
        End
    }
}