using System.Collections.Generic;
using MrCMS.Web.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Core.Models.Widgets;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Widgets;

public class SliderWidgetModel : IUpdatePropertiesViewModel<SliderWidget>, IAddPropertiesViewModel<SliderWidget>
{
    public SliderWidgetModel()
    {
        Interval = 5000;
        ShowIndicator = true;
    }
    public List<SlideViewModel> SlideList { get; set; }
    
    public int Interval { get; set; }

    public bool ShowIndicator { get; set; }

    public bool PauseOnHover { get; set; }

    public override string ToString()
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject((SlideList ?? new List<SlideViewModel>()));
    }
}