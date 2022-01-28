using MrCMS.Web.Admin.Infrastructure.Services.Content;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class SliderAdminConfiguration : ContentBlockAdminConfigurationBase<Slider, UpdateSliderModel> 
{
    public override UpdateSliderModel GetEditModel(Slider block)
    {
        return new UpdateSliderModel();
    }

    public override void UpdateBlock(Slider block, UpdateSliderModel editModel)
    {
    }
}