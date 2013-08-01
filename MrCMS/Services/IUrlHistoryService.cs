using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IUrlHistoryService
    {
        void Delete(UrlHistory urlHistory);
        void Add(UrlHistory urlHistory);
        IEnumerable<UrlHistory> GetAllOtherUrls(Webpage document);
        UrlHistory GetByUrlSegment(string url);
    }
}