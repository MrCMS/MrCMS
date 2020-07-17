using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class UpdatePageTemplateModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DisplayName("Page View Name (e.g. TextPage)")]
        public string PageTemplateName { get; set; }

        [Required]
        public string PageType { get; set; }

        public int? LayoutId { get; set; }

        [Required]
        [DisplayName("URL Generator Type")]
        public string UrlGeneratorType { get; set; }

        [DisplayName("Single Use?")]
        public bool SingleUse { get; set; }
    }
}