using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class GetDocumentsByParent<T> : IGetDocumentsByParent<T> where T : Document
    {
        private readonly IRepository<T> _repository;

        public GetDocumentsByParent(IRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<T>> GetDocuments(T parent)
        {
            var queryable = _repository.Query();
            queryable = parent != null
                ? queryable.Where(arg => arg.Parent.Id == parent.Id)
                : queryable
                    .Where(arg => arg.Parent == null);
            return await queryable.OrderBy(arg => arg.DisplayOrder)
                .ToListAsync();
        }
    }
}