using System.Linq;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
using NHibernate;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class BatchAdminService : IBatchAdminService
    {
        private readonly ISession _session;

        public BatchAdminService(ISession session)
        {
            _session = session;
        }

        public async Task<IPagedList<Batch>> Search(BatchSearchModel searchModel)
        {
            return await _session.Query<Batch>().OrderByDescending(batch => batch.Id).PagedAsync(searchModel.Page);
        }

        public async Task<Batch> Get(int id)
        {
            return await _session.GetAsync<Batch>(id);
        }
    }
}