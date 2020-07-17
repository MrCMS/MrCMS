using MrCMS.Batching.Entities;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IBatchAdminService
    {
        IPagedList<Batch> Search(BatchSearchModel searchModel);
    }
}