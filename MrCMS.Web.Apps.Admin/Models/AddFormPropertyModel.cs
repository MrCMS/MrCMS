namespace MrCMS.Web.Apps.Admin.Models
{
    public class AddFormPropertyModel
    {
        public int FormId { get; set; }
        public string PropertyType { get; set; }
        public bool Required { get; set; }

        public string Name { get; set; }
        public string LabelText { get; set; }
        public string CssClass { get; set; }
        public string HtmlId { get; set; }
    }
}