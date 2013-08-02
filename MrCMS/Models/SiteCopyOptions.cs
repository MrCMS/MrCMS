using System.ComponentModel;

namespace MrCMS.Models
{
    public class SiteCopyOptions
    {
        [DisplayName("Copy settings from another site?")]
        public int? SiteId { get; set; }

        [DisplayName("Also copy layouts?")]
        public bool CopyLayouts { get; set; }
        [DisplayName("Also copy media categories?")]
        public bool CopyMediaCategories { get; set; }
        [DisplayName("Also copy home page?")]
        public bool CopyHome { get; set; }
        [DisplayName("Also copy 404?")]
        public bool Copy404 { get; set; }
        [DisplayName("Also copy 403?")]
        public bool Copy403 { get; set; }
        [DisplayName("Also copy 500?")]
        public bool Copy500 { get; set; }
    }
}