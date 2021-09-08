using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Models;
using NHibernate;

namespace MrCMS.Services
{
    public class SortService : ISortService
    {
        private readonly ISession _session;

        public SortService(ISession session)
        {
            _session = session;
        }

        public async Task Sort<T>(IList<SortItem> sortItems) where T : SiteEntity, ISortable
        {
            await _session.TransactAsync(async session =>
            {

                foreach (var item in sortItems)
                {
                    var entity = await session.GetAsync<T>(item.Id);
                    entity.DisplayOrder = item.Order;
                    await session.UpdateAsync(entity);
                    
                }
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