using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IUrlHistoryAdminService
    {
        void Delete(UrlHistory urlHistory);
        void Add(UrlHistory urlHistory);
        UrlHistory GetByUrlSegment(string url);
        UrlHistory GetUrlHistoryToAdd(int webpageId);
    }
}