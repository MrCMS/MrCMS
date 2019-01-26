using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class AddStringResourceModel
    {
        [Required]
        public string Value { get; set; }
        public string Key { get; set; }
        public int? SiteId { get; set; }
        public string UICulture { get; set; }
    }
}