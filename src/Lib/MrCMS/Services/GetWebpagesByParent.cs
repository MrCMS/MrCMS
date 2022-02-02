using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class GetWebpagesByParent : IGetWebpagesByParent
    {
        private readonly IRepository<Webpage> _repository;

        public GetWebpagesByParent(IRepository<Webpage> repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<Webpage>> GetWebpages(Webpage parent)
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