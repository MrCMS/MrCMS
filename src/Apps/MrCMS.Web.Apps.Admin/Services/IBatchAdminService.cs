using MrCMS.Batching.Entities;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IBatchAdminService
    {
        IPagedList<Batch> Search(BatchSearchModel searchModel);
    }
}