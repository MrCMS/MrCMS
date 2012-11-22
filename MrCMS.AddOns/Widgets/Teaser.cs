using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;

namespace MrCMS.AddOns.Widgets
{
    [MrCMSMapClass]
    public class Teaser : Widget
    {
        public virtual string Title { get; set; }
        public virtual string Image { get; set; }
        public virtual string Text { get; set; }
        public virtual string Url { get; set; }
    }
}