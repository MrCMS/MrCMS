using MrCMS.Web.Admin.Infrastructure.Services.Content;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class ImageAdminConfiguration : ContentBlockAdminConfigurationBase<Image, UpdateImageAdminModel>
{
    public override UpdateImageAdminModel GetEditModel(Image block)
    {
        return new UpdateImageAdminModel { Aligment = block.Aligment, cssClasses = block.CssClasses, Url = block.Url };
    }

    public override void UpdateBlock(Image block, UpdateImageAdminModel editModel)
    {
        block.Aligment = editModel.Aligment;
        block.CssClasses = editModel.cssClasses;
        block.Url = editModel.Url;
    }
}