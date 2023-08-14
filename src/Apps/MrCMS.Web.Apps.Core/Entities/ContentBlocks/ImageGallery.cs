using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Entities.BlockItems;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Core.Entities.ContentBlocks;

public class ImageGallery : IContentBlockWithChildCollection
{
    public string DisplayName => $"Image Gallery ({Images.Count} images)";
    public IReadOnlyList<BlockItem> Items => Images;
    public List<ImageGalleryItem> Images { get; set; } = new();

    public string ResponsiveClasses { get; set; } = "col-sm-6 col-md-4 col-lg-3 col-xl-2";
    public string ImageRatio { get; set; } = "7x9";
    public int ImageRenderSize { get; set; } = 250;

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

