using MrCMS.Entities.Documents.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

[Display(Name = "Image")]
public class Image : IContentBlock
{
    public Image()
    {
        Aligment = ImageAligment.Center;
        cssClasses = "img-fluid rounded";
    }
    public string Url { get; set; }
    public ImageAligment Aligment { get; set; }
    public string cssClasses { get; set; }

    public IReadOnlyList<BlockItem> Items => new BlockItem[] { };

    public enum ImageAligment
    {
        Start,
        Center,
        End
    }
}