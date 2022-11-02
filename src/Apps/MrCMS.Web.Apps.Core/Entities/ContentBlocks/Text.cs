using MrCMS.Entities.Documents.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

[Display(Name = "Text")]
public class Text : IContentBlock
{
    public Text()
    {
        HeadingAligment = Aligment.Start;
    }

    public string Heading { get; set; }
    public Aligment HeadingAligment { get; set; }
    public string Subtext { get; set; }

    public IReadOnlyList<BlockItem> Items => new BlockItem[] { };

    public enum Aligment
    {
        Start,
        Center,
        End
    }
}