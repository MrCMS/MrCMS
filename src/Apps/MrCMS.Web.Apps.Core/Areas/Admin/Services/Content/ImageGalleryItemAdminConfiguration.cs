using MrCMS.Web.Admin.Infrastructure.Services.Content;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.BlockItems;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class ImageGalleryItemAdminConfiguration : BlockItemAdminConfigurationBase<ImageGalleryItem, UpdateImageGalleryItemModel>
{
    public override UpdateImageGalleryItemModel GetEditModel(ImageGalleryItem block)
    {
        return new UpdateImageGalleryItemModel
        {
            Url = block.Url
        };
    }

    public override void UpdateBlockItem(ImageGalleryItem block, UpdateImageGalleryItemModel editModel)
    {
        block.Url = editModel.Url;
    }
}