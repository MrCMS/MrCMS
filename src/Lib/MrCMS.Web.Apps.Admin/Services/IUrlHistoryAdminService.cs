using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IUrlHistoryAdminService
    {
        Task<UrlHistory> Delete(int id);
        Task Add(AddUrlHistoryModel urlHistory);
        UrlHistory GetByUrlSegment(string url);
        AddUrlHistoryModel GetUrlHistoryToAdd(int webpageId);
        Task<List<UrlHistory>> GetByWebpageId(int webpageId);
    }
}