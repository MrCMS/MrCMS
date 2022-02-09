using MrCMS.Web.Admin.Infrastructure.Services.Content;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class LineBreakAdminConfiguration : ContentBlockAdminConfigurationBase<LineBreak, UpdateLineBreakAdminModel>
{
    public override UpdateLineBreakAdminModel GetEditModel(LineBreak block)
    {
        return new UpdateLineBreakAdminModel { CssClasses = block.CssClasses };
    }

    public override void UpdateBlock(LineBreak block, UpdateLineBreakAdminModel editModel)
    {
        block.CssClasses = editModel.CssClasses;
    }
}