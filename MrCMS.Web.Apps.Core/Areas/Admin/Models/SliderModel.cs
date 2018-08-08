using MrCMS.Web.Apps.Admin.Models.Tabs;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models
{
    public class SliderModel : IUpdatePropertiesViewModel<Slider>, IAddPropertiesViewModel<Slider>
    {
        public string Image { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }

        public string Image1 { get; set; }
        public string Link1 { get; set; }
        public string Description1 { get; set; }

        public string Image2 { get; set; }
        public string Link2 { get; set; }
        public string Description2 { get; set; }

        public string Image3 { get; set; }
        public string Link3 { get; set; }
        public string Description3 { get; set; }
    }
}