using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IBatchAdminService
    {
        Task<IPagedList<Batch>> Search(BatchSearchModel searchModel);
        Task<Batch> Get(int id);
    }
}