namespace MrCMS.Services
{
    public interface IUrlValidationService
    {
        bool UrlIsValidForMediaCategory(string urlSegment, int? id);
        bool UrlIsValidForLayout(string urlSegment, int? id);
        bool UrlIsValidForWebpage(string url, int? id);
        bool UrlIsValidForWebpageUrlHistory(string url);
    }
}