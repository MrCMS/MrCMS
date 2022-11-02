using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Entities.BlockItems;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

[Display(Name = "Image Gallery")]
public class ImageGallery : IContentBlockWithChildCollection
{
    public ImageGallery()
    {
        ResponsiveClasses = "col-sm-6 col-md-4 col-lg-3 col-xl-2";
        ImageRatio = "7x9";
        ImageRenderSize = 250;
    }
    public IReadOnlyList<BlockItem> Items => Images;
    public List<ImageGalleryItem> Images { get; set; } = new();

    public string ResponsiveClasses { get; set; }
    public string ImageRatio { get; set; }
    public int ImageRenderSize { get; set; }

    public BlockItem AddChild()
    {
        var image = new ImageGalleryItem();
        Images.Add(image);
        return image;
    }

    public void Remove(BlockItem item)
    {
        Images.Remove(item as ImageGalleryItem);
    }
}

