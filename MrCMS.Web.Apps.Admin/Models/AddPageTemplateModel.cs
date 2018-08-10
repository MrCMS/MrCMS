namespace MrCMS.Web.Apps.Admin.Models
{
    public class AddPageTemplateModel
    {
        public string Name { get; set; }
        public string PageTemplateName { get; set; }
        public string PageType { get; set; }
        public int? LayoutId { get; set; }
        public string UrlGeneratorType { get; set; }
        public bool SingleUse { get; set; }
    }
}