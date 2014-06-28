using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IUrlHistoryAdminService
    {
        void Delete(UrlHistory urlHistory);
        void Add(UrlHistory urlHistory);
        UrlHistory GetByUrlSegment(string url);
        UrlHistory GetByUrlSegmentWithSite(string url, Site site, Webpage page);
        UrlHistory GetUrlHistoryToAdd(int webpageId);
    }
}