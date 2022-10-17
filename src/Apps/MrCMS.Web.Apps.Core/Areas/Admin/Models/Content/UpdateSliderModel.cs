using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Content;

public class UpdateSliderModel
{
    [DisplayName("Slider interval"), Range(500, int.MaxValue)]
    public int Interval { get; set; }

    [DisplayName("Show slider indicator")]
    public bool ShowIndicator { get; set; }

    [DisplayName("Pause slider on hover")]
    public bool PauseOnHover { get; set; }
    
    [DisplayName("Caption Css Class")]
    public string CaptionCssClass { get; set; }
}