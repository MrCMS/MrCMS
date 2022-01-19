using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web.ContentAreas;
using Newtonsoft.Json;

namespace MrCMS.Entities.Documents.Web.ContentRows;

[ContentBlockMetadata("Image and Text", "image-and-text")]
public class ImageAndText : IContentBlock
{
    public ImageAndTextLayout Layout { get; set; }

    public IReadOnlyList<BlockItem> Items => new BlockItem[] { Image, Text };

    public ContentImage Image { get; set; }
    
    public ContentText Text { get; set; }
    
    public enum ImageAndTextLayout
    {
        ImageLeft,
        ImageRight
    }
}