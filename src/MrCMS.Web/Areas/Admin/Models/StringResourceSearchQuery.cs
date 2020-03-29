using System.ComponentModel;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class StringResourceSearchQuery
    {
        public StringResourceSearchQuery()
        {
            Page = 1;
        }
        public int Page { get; set; }
        [DisplayName("Site")]
        public int? SiteId { get; set; }
        public string Language { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

    }
}