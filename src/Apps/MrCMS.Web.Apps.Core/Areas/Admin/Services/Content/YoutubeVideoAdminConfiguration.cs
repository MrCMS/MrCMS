using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.BlockItems;
using MrCMS.Web.Admin.Infrastructure.Services.Content;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class YoutubeVideoAdminConfiguration : BlockItemAdminConfigurationBase<YoutubeVideo, UpdateYoutubeVideoAdminModel>
{
    public override UpdateYoutubeVideoAdminModel GetEditModel(YoutubeVideo block)
    {
        return new UpdateYoutubeVideoAdminModel
        {
            Autoplay = block.Autoplay,
            EnablePrivacyMode = block.EnablePrivacyMode,
            ShowPlayerControl = block.ShowPlayerControl,
            StartAt = block.StartAt,
            VideoId = block.VideoId
        };
    }

    public override void UpdateBlockItem(YoutubeVideo block, UpdateYoutubeVideoAdminModel editModel)
    {
        block.VideoId = editModel.VideoId;
        block.EnablePrivacyMode = editModel.EnablePrivacyMode;
        block.StartAt = editModel.StartAt;
        block.ShowPlayerControl = editModel.ShowPlayerControl;
        block.Autoplay = editModel.Autoplay;
    }
}