using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IUrlHistoryAdminService
    {
        Task<UrlHistory> Delete(int id);
        Task Add(AddUrlHistoryModel urlHistory);
        AddUrlHistoryModel GetUrlHistoryToAdd(int webpageId);
    }
}