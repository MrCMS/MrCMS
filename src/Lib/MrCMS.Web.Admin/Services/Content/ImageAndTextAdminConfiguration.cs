using MrCMS.Entities.Documents.Web.ContentBlocks;
using MrCMS.Web.Admin.Models.Content;

namespace MrCMS.Web.Admin.Services.Content;

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