using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.BlockItems;
using MrCMS.Web.Admin.Infrastructure.Services.Content;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

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