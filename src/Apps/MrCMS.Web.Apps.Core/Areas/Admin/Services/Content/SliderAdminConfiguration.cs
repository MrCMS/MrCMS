using MrCMS.Web.Admin.Infrastructure.Services.Content;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;
using MrCMS.Web.Apps.Core.Entities.ContentBlocks;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Services.Content;

public class SliderAdminConfiguration : ContentBlockAdminConfigurationBase<Slider, UpdateSliderModel>
{
    public override UpdateSliderModel GetEditModel(Slider block)
    {
        return new UpdateSliderModel
        {
            Interval = block.Interval, PauseOnHover = block.PauseOnHover, ShowIndicator = block.ShowIndicator,
            CaptionCssClass = block.CaptionCssClass
        };
    }

    public override void UpdateBlock(Slider block, UpdateSliderModel editModel)
    {
        block.Interval = editModel.Interval;
        block.ShowIndicator = editModel.ShowIndicator;
        block.PauseOnHover = editModel.PauseOnHover;
        block.CaptionCssClass = editModel.CaptionCssClass;
    }
}