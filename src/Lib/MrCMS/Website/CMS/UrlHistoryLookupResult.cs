namespace MrCMS.Website.CMS
{
    public class UrlHistoryLookupResult
    {
        public string RedirectUrl { get; set; }
        public bool Match => !string.IsNullOrWhiteSpace(RedirectUrl);
    }
}