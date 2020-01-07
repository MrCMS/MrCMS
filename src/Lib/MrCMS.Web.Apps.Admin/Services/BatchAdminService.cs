using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Data;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Models;

using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class BatchAdminService : IBatchAdminService
    {
        private readonly IRepository<Batch> _repository;

        public BatchAdminService(IRepository<Batch> repository)
        {
            _repository = repository;
        }

        public IPagedList<Batch> Search(BatchSearchModel searchModel)
        {
            return _repository.Readonly().OrderByDescending(batch => batch.Id).ToPagedList(searchModel.Page);
        }
    }
}