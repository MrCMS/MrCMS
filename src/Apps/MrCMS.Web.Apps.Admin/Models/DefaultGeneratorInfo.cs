using System.ComponentModel;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class DefaultsInfo
    {
        public string PageTypeName { get; set; }
        public string PageTypeDisplayName { get; set; }
        [DisplayName("Generator")]
        public string GeneratorTypeName { get; set; }

        [DisplayName("Layout")]
        public int? LayoutId { get; set; }
    }
}