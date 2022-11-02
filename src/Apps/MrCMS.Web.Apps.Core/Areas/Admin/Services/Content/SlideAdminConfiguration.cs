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
            Url = block.Url,
            Caption = block.Caption,
            MobileImageUrl = block.MobileImageUrl
        };
    }

    public override void UpdateBlockItem(Slide block, UpdateSlideAdminModel editModel)
    {
        block.Url = editModel.Url;
        block.MobileImageUrl = editModel.MobileImageUrl;
        block.Caption = editModel.Caption;
    }
}