using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
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