using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class Dashboard
    {
        public string LoggedInName { get; set; }
        public string SiteName { get; set; }
        public IList<WebpageStats> Stats { get; set; }
        public int ActiveUsers { get; set; }
        public int NoneActiveUsers { get; set; }
    }

    public class WebpageStats
    {
        public string DocumentType { get; set; }
        public int NumberOfPages { get; set; }
        public int NumberOfUnPublishedPages { get; set; }
    }
}