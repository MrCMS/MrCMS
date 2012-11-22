using MrCMS.DbConfiguration.Mapping;

namespace MrCMS.Entities.Widget
{
    [MrCMSMapClass]
    public class PlainTextWidget : Widget
    {
        public virtual string Text { get; set; }
    }
}