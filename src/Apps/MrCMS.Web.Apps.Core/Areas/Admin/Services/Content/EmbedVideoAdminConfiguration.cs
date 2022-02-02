using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.BlockItems;
using MrCMS.Web.Admin.Infrastructure.Services.Content;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class EmbedVideoAdminConfiguration : BlockItemAdminConfigurationBase<EmbedVideo, UpdateEmbedVideoAdminModel>
{
    public override UpdateEmbedVideoAdminModel GetEditModel(EmbedVideo block)
    {
        return new UpdateEmbedVideoAdminModel
        {
            EmbedCode = block.EmbedCode
        };
    }

    public override void UpdateBlockItem(EmbedVideo block, UpdateEmbedVideoAdminModel editModel)
    {
        block.EmbedCode = editModel.EmbedCode;
    }
}