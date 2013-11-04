using System.ComponentModel;

namespace MrCMS.Models
{
    public class SiteCopyOptions
    {
        [DisplayName("Copy settings from another site?")]
        public int? SiteId { get; set; }

        [DisplayName("Copy layouts?")]
        public bool CopyLayouts { get; set; }
        [DisplayName("Copy media categories?")]
        public bool CopyMediaCategories { get; set; }
        [DisplayName("Copy home page?")]
        public bool CopyHome { get; set; }
        [DisplayName("Copy 404 page?")]
        public bool Copy404 { get; set; }
        [DisplayName("Copy 403 page?")]
        public bool Copy403 { get; set; }
        [DisplayName("Copy 500 page?")]
        public bool Copy500 { get; set; }
        [DisplayName("Copy Login Page?")]
        public bool CopyLogin { get; set; }
    }
}