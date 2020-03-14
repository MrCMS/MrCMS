using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public class GetDocumentsByParent<T> : IGetDocumentsByParent<T> where T : Document
    {
        private readonly IRepository<T> _repository;

        public GetDocumentsByParent(IRepository<T> repository)
        {
            _repository = repository;
        }

        public Task<List<T>> GetDocuments(T parent)
        {
            var queryable = _repository.Query();
            return GetDocumentByParent(queryable, parent);
        }

        public static Task<List<T>> GetDocumentByParent(IQueryable<T> queryable, T parent)
        {
            queryable = parent != null
                ? queryable.Where(arg => arg.ParentId == parent.Id)
                : queryable
                    .Where(arg => arg.Parent == null);
            return queryable.OrderBy(arg => arg.DisplayOrder)
                .ToListAsync();
        }
    }
}