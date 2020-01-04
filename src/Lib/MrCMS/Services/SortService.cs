using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Models;

namespace MrCMS.Services
{
    public class SortService : ISortService
    {
        private readonly IRepositoryResolver _repositoryResolver;

        public SortService(IRepositoryResolver repositoryResolver)
        {
            _repositoryResolver = repositoryResolver;
        }

        public async Task Sort<T>(IList<SortItem> sortItems) where T : SiteEntity, ISortable
        {
            await _repositoryResolver.GetRepository<T>().Transact(async (repo, ct) =>
            {
                foreach (var item in sortItems)
                {
                    var entity = await repo.Load(item.Id, ct);
                    entity.DisplayOrder = item.Order;
                    await repo.Update(entity, ct);
                };
            });
        }

        public IList<SortItem> GetSortItems<T>(List<T> items) where T : SiteEntity, ISortable
        {
            return items.Select(arg => new SortItem
            {
                Order = arg.DisplayOrder,
                Name = arg.DisplayName,
                Id = arg.Id
            }).ToList();
        }
    }
}