using MrCMS.Entities.Documents.Web.BlockItems;
using MrCMS.Web.Admin.Models.Content;

namespace MrCMS.Web.Admin.Services.Content;

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