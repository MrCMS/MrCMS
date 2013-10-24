using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services
{
    public interface IUrlHistoryService
    {
        void Delete(UrlHistory urlHistory);
        void Add(UrlHistory urlHistory);
        IEnumerable<UrlHistory> GetAllOtherUrls(Webpage document);
        UrlHistory GetByUrlSegment(string url);
        UrlHistory GetByUrlSegmentWithSite(string url, Site site, Webpage page);
    }
}