using MrCMS.Web.Admin.Infrastructure.Services.Content;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.BlockItems;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class SlideAdminConfiguration : BlockItemAdminConfigurationBase<Slide, UpdateSlideAdminModel>
{
    public override UpdateSlideAdminModel GetEditModel(Slide block)
    {
        return new UpdateSlideAdminModel
        {
            Image = block.Image,
            Caption = block.Caption,
            MobileImage = block.MobileImage,
            Url = block.Image
        };
    }

    public override void UpdateBlockItem(Slide block, UpdateSlideAdminModel editModel)
    {
        block.Image = editModel.Image;
        block.MobileImage = editModel.MobileImage;
        block.Caption = editModel.Caption;
        block.Image = editModel.Url;
    }
}