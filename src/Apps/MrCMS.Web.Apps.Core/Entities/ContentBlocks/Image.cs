using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

public class Image : IContentBlock
{
    public string DisplayName => "Image";
    public string Url { get; set; }
    public ImageAligment Aligment { get; set; } = ImageAligment.Center;
    public string CssClasses { get; set; } = "img-fluid rounded";

    public IReadOnlyList<BlockItem> Items => new BlockItem[] { };

    public enum ImageAligment
    {
        Start,
        Center,
        End
    }
}