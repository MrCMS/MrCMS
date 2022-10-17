using System.Collections.Generic;
using System.ComponentModel;
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
        CaptionCssClass = "d-none d-md-block";
    }
    public List<SlideViewModel> SlideList { get; set; }
    
    public int Interval { get; set; }

    public bool ShowIndicator { get; set; }

    public bool PauseOnHover { get; set; }
    
    [DisplayName("Caption Css Class")]
    public string CaptionCssClass { get; set; }

    public override string ToString()
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject((SlideList ?? new List<SlideViewModel>()));
    }
}