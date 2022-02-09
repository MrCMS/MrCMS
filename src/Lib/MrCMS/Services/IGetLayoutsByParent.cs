using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using NHibernate.Linq;

namespace MrCMS.Services;

public interface IGetLayoutsByParent
{
    Task<IReadOnlyList<Layout>> GetLayouts(Layout parent);
}

public class GetLayoutsByParent : IGetLayoutsByParent
{
    private readonly IRepository<Layout> _repository;

    public GetLayoutsByParent(IRepository<Layout> repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<Layout>> GetLayouts(Layout parent)
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