using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class AddPageTemplateModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DisplayName("Page View Name (e.g. TextPage)")]
        public string PageTemplateName { get; set; }

        [Required]
        public string PageType { get; set; }

        [DisplayName("Layout")]
        public int? LayoutId { get; set; }

        [Required]
        [DisplayName("URL Generator Type")]
        public string UrlGeneratorType { get; set; }

        [DisplayName("Single Use?")]
        public bool SingleUse { get; set; }
    }
    
    public class PageTemplateModel
    {   
        public int Id { get; set; }
        public string Name { get; set; }

        public string PageTemplateName { get; set; }

        public string PageType { get; set; }

        public string LayoutName { get; set; }
        public string UrlGeneratorType { get; set; }

        public bool SingleUse { get; set; }
        
        public int Count { get; set; }
    }
    
    
}