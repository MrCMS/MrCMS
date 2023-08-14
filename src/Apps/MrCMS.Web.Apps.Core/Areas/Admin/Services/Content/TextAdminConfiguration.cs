using MrCMS.Web.Admin.Infrastructure.Services.Content;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class TextAdminConfiguration : ContentBlockAdminConfigurationBase<Text, UpdateTextAdminModel>
{
    public override UpdateTextAdminModel GetEditModel(Text block)
    {
        return new UpdateTextAdminModel { Heading = block.Heading, HeadingAlignment = block.HeadingAlignment, Subtext = block.Subtext };
    }

    public override void UpdateBlock(Text block, UpdateTextAdminModel editModel)
    {
        block.Heading = editModel.Heading;
        block.HeadingAlignment = editModel.HeadingAlignment;
        block.Subtext = editModel.Subtext;
    }
}