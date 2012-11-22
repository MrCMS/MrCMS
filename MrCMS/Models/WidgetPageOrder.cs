namespace MrCMS.Models
{
    public class WidgetPageOrder
    {
        public int WidgetId { get; set; }
        public int WebpageId { get; set; }
        public int LayoutAreaId { get; set; }
        public int Order { get; set; }
    }
}