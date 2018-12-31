namespace MrCMS.Services.Resources
{
    public class LocalizationInfo
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Culture { get; set; }
        public int? SiteId { get; set; }
    }
}