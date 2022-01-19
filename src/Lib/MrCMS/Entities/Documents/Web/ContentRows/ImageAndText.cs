using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web.ContentAreas;
using Newtonsoft.Json;

namespace MrCMS.Entities.Documents.Web.ContentRows;

[ContentRowMetadata("Image and Text", "image-and-text")]
public class ImageAndText : IContentRow
{
    public ImageAndTextLayout Layout { get; set; }

    public IReadOnlyList<ContentArea> Areas => new ContentArea[] { Image, Text };

    public ContentImage Image { get; set; }
    
    public ContentText Text { get; set; }
    
    public enum ImageAndTextLayout
    {
        ImageLeft,
        ImageRight
    }
    
    // public class ImageAndTextData
    // {
    //     public ImageAndTextLayout Layout { get; set; }
    //     public ContentImage Image { get; set; }
    //     public ContentText Text { get; set; }
    // }

}