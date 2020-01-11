using MrCMS.Data;

namespace MrCMS.Entities.Documents.Web
{
    public class HiddenWidget : IJoinTable
    {
        public Webpage Webpage { get; set; }
        public int WebpageId { get; set; }
        public Widget.Widget Widget { get; set; }
        public int WidgetId { get; set; }
    }
}