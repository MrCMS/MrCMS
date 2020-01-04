using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Models;

using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class BatchAdminService : IBatchAdminService
    {
        private readonly ISession _session;

        public BatchAdminService(ISession session)
        {
            _session = session;
        }

        public IPagedList<Batch> Search(BatchSearchModel searchModel)
        {
            return _session.Query<Batch>().OrderByDescending(batch => batch.Id).ToPagedList(searchModel.Page);
        }
    }
}