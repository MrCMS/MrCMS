using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MrCMS.Entities.Documents.Web.ContentAreas;
using Newtonsoft.Json;

namespace MrCMS.Entities.Documents.Web.ContentRows;

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