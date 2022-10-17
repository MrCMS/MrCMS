using MrCMS.Web.Admin.Infrastructure.Services.Content;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class
    ImageGalleryAdminConfiguration : ContentBlockAdminConfigurationBase<ImageGallery, UpdateImageGalleryAdminModel>
{
    public override UpdateImageGalleryAdminModel GetEditModel(ImageGallery block)
    {
        return new UpdateImageGalleryAdminModel
        {
            ResponsiveClasses = block.ResponsiveClasses, ImageRatio = block.ImageRatio,
            ImageRenderSize = block.ImageRenderSize
        };
    }

    public override void UpdateBlock(ImageGallery block, UpdateImageGalleryAdminModel editModel)
    {
        block.ResponsiveClasses = editModel.ResponsiveClasses;
        block.ImageRatio = editModel.ImageRatio;
        block.ImageRenderSize = editModel.ImageRenderSize;
    }
}