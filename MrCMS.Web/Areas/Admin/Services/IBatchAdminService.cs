using MrCMS.Batching.Entities;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IBatchAdminService
    {
        IPagedList<Batch> Search(BatchSearchModel searchModel);
    }
}