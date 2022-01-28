using MrCMS.Web.Admin.Infrastructure.Services.Content;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class ImageAndTextAdminConfiguration : ContentBlockAdminConfigurationBase<ImageAndText, UpdateImageAndTextModel> 
{
    public override UpdateImageAndTextModel GetEditModel(ImageAndText block)
    {
        return new UpdateImageAndTextModel { Layout = block.Layout };
    }

    public override void UpdateBlock(ImageAndText block, UpdateImageAndTextModel editModel)
    {
        block.Layout = editModel.Layout;
    }
}