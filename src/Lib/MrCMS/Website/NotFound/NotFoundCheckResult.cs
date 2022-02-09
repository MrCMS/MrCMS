using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.NotFound
{
    public class NotFoundCheckResult
    {
        public bool Success { get; }
        public string Url { get; }
        public Webpage Webpage { get; }

        private NotFoundCheckResult(bool success, string url, Webpage webpage)
        {
            Success = success;
            Url = url;
            Webpage = webpage;
        }

        public static readonly NotFoundCheckResult NotFound = new(false, null, null);

        public static NotFoundCheckResult ForUrl(string url)
        {
            return new(true, url, null);
        }

        public static NotFoundCheckResult ForWebpage(Webpage webpage)
        {
            return new(true, null, webpage);
        }

        public string GetRedirectUrl()
        {
            if (!Success)
                return null;

            if (!string.IsNullOrWhiteSpace(Url))
                return Url;
            
            return $"/{Webpage.UrlSegment}";
        }
    }
}