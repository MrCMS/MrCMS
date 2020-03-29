using System;

namespace MrCMS.Web.Areas.Admin.Models.Search
{
    public class AdminSearchQuery
    {
        public AdminSearchQuery()
        {
            Page = 1;
        }

        public int Page { get; set; }

        public string Term { get; set; }
        public string Type { get; set; }

        public DateTime? CreatedOnFrom { get; set; }
        public DateTime? CreatedOnTo { get; set; }
    }
}