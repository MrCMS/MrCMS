using System;
using System.ComponentModel;

namespace MrCMS.Web.Admin.Models.Search
{
    public class AdminWebpageSearchQuery
    {
        public AdminWebpageSearchQuery()
        {
            Page = 1;
        }
        public string Term { get; set; }
        public int Page { get; set; }
        public int? ParentId { get; set; }
        public string Type { get; set; }
        [DisplayName("Created On From")]
        public DateTime? CreatedOnFrom { get; set; }
        [DisplayName("Created On To")]
        public DateTime? CreatedOnTo { get; set; }

    }
}