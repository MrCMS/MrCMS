namespace MrCMS.Web.Apps.Admin.Models
{
    public class UpdateFormPropertyModel
    {
        public int Id { get; set; }
        public bool Required { get; set; }

        public string Name { get; set; }
        public string LabelText { get; set; }
        public string CssClass { get; set; }
        public string HtmlId { get; set; }

        public bool ShowPlaceholder { get; set; }
        public string Placeholder { get; set; }
    }
}