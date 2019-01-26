using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface ISortService
    {
        void Sort<T>(IList<SortItem> sortItems) where T : SiteEntity, ISortable;
        IList<SortItem> GetSortItems<T>(List<T> items) where T : SiteEntity, ISortable;
    }
}