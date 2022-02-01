using MrCMS.Web.Admin.Infrastructure.Services.Content;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class VideoAdminConfiguration : ContentBlockAdminConfigurationBase<Video, UpdateVideoAdminModel>
{
    public override UpdateVideoAdminModel GetEditModel(Video block)
    {
        return new UpdateVideoAdminModel
        {
            ActiveType = block.ActiveType,
        };
    }

    public override void UpdateBlock(Video block, UpdateVideoAdminModel editModel)
    {
        block.ActiveType = editModel.ActiveType;
    }
}