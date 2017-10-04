namespace MrCMS.Web.Areas.Admin.Models
{
    public class PageDefaultsInfo
    {
        public string TypeName { get; set; }
        public string DisplayName { get; set; }
        public string GeneratorDisplayName { get; set; }
        public string LayoutName { get; set; }
        public CacheEnabledStatus CacheEnabled { get; set; }
    }

    public enum CacheEnabledStatus
    {
        Enabled,
        Disabled,
        Unavailable
    }
}