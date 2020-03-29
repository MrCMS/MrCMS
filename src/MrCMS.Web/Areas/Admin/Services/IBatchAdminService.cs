using MrCMS.Batching.Entities;
using MrCMS.Web.Areas.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IBatchAdminService
    {
        IPagedList<Batch> Search(BatchSearchModel searchModel);
    }
}