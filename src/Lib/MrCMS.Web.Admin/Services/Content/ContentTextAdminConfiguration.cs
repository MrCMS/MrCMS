using MrCMS.Entities.Documents.Web.BlockItems;
using MrCMS.Web.Admin.Models.Content;

namespace MrCMS.Web.Admin.Services.Content;

public class ContentTextAdminConfiguration : BlockItemAdminConfigurationBase<ContentText, UpdateContentTextAdminModel> 
{
    public override UpdateContentTextAdminModel GetEditModel(ContentText block)
    {
        return new UpdateContentTextAdminModel
        {
            Text = block.Text
        };
    }

    public override void UpdateBlockItem(ContentText block, UpdateContentTextAdminModel editModel)
    {
        block.Text = editModel.Text;
    }
}