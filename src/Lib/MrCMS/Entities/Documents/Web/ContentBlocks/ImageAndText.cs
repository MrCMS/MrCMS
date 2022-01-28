using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Documents.Web.BlockItems;

namespace MrCMS.Entities.Documents.Web.ContentBlocks;

[Display( Name="Image and Text")]
public class ImageAndText : IContentBlock
{
    public ImageAndTextLayout Layout { get; set; }

    public IReadOnlyList<BlockItem> Items => new BlockItem[] { Image, Text };

    public ContentImage Image { get; set; } = new() { Name = "Image" };

    public ContentText Text { get; set; } = new() { Name = "Text" };

    public enum ImageAndTextLayout
    {
        ImageLeft,
        ImageRight
    }
}