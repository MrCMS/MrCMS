using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface ISortService
    {
        Task Sort<T>(IList<SortItem> sortItems) where T : SiteEntity, ISortable;
        IList<SortItem> GetSortItems<T>(List<T> items) where T : SiteEntity, ISortable;
    }
}