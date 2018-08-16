using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IGetLiveUrl
    {
        string GetUrlSegment(Webpage webpage, bool addLeadingSlash = false);
        string GetAbsoluteUrl(Webpage webpage);
    }
}