using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Areas.Admin.Services
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