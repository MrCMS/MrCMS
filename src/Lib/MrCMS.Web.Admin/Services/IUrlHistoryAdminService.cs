using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IUrlHistoryAdminService
    {
        UrlHistory Delete(int id);
        void Add(AddUrlHistoryModel urlHistory);
        UrlHistory GetByUrlSegment(string url);
        UrlHistory GetByUrlSegmentWithSite(string url, Site site, Webpage page);
        AddUrlHistoryModel GetUrlHistoryToAdd(int webpageId);
    }
}