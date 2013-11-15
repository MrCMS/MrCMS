using System.ComponentModel;

namespace MrCMS.Logging
{
    public class LogSearchQuery
    {
        public LogSearchQuery()
        {
            Page = 1;
        }
        public int Page { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        [DisplayName("Site")]
        public int? SiteId { get; set; }
        [DisplayName("Filter logs")]
        public LogEntryType? Type { get; set; }
    }
}