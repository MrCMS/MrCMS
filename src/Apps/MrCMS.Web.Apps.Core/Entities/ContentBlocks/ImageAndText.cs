using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Entities.BlockItems;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

public class ImageAndText : IContentBlock
{
    
    public string DisplayName => "Image and Text";
    public ImageAndTextLayout Layout { get; set; }
    public VerticalAlignment ContentVerticalAlignment { get; set; }

    public IReadOnlyList<BlockItem> Items => new BlockItem[] { Image, Text };

    public ContentImage Image { get; set; } = new() { Name = "Image" };

    public ContentText Text { get; set; } = new() { Name = "Text" };

    public enum ImageAndTextLayout
    {
        ImageStart,
        ImageEnd
    }
    
    public enum VerticalAlignment
    {
        Start,
        Center,
        End
    }
}