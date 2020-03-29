using MrCMS.Website.Caching;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class UpdateWidgetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRecursive { get; set; }
        public bool Cache { get; set; }
        public string CacheLength { get; set; }
        public CacheExpiryType CacheExpiryType { get; set; }
    }
}