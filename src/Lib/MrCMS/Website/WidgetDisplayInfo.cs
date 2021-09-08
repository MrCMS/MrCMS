using System.Collections.Generic;

namespace MrCMS.Website
{
    public class WidgetDisplayInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<WidgetData> Widgets { get; set; }
    }
}