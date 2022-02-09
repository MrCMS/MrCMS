using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using NHibernate.Linq;

namespace MrCMS.Services;

public class GetMediaCategoriesByParent : IGetMediaCategoriesByParent
{
    private readonly IRepository<MediaCategory> _repository;

    public GetMediaCategoriesByParent(IRepository<MediaCategory> repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MediaCategory>> GetMediaCategories(MediaCategory parent)
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