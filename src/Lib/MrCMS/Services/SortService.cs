using System.Collections.Generic;
using System.Linq;
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

        public void Sort<T>(IList<SortItem> sortItems) where T : SiteEntity, ISortable
        {
            _session.Transact(session => sortItems.ForEach(item =>
            {
                var entity = session.Get<T>(item.Id);
                entity.DisplayOrder = item.Order;
                session.Update(entity);
            }));
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