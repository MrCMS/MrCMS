using MrCMS.Web.Admin.Infrastructure.Services.Content;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.BlockItems;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class ContentImageAdminConfiguration : BlockItemAdminConfigurationBase<ContentImage, UpdateContentImageAdminModel> 
{
    public override UpdateContentImageAdminModel GetEditModel(ContentImage block)
    {
        return new UpdateContentImageAdminModel
        {
            Url = block.Url
        };
    }

    public override void UpdateBlockItem(ContentImage block, UpdateContentImageAdminModel editModel)
    {
        block.Url = editModel.Url;
    }
}