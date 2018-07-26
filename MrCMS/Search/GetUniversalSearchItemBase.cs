using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Search.Models;

namespace MrCMS.Search
{
    public abstract class GetUniversalSearchItemBase
    {
        public abstract UniversalSearchItem GetSearchItem(SystemEntity entity);
        public abstract HashSet<UniversalSearchItem> GetSearchItems(HashSet<SystemEntity> entities);
    }

    public abstract class GetUniversalSearchItemBase<T> : GetUniversalSearchItemBase where T : SystemEntity
    {
        public abstract UniversalSearchItem GetSearchItem(T entity);

        public override UniversalSearchItem GetSearchItem(SystemEntity entity)
        {
            var t = entity as T;
            return t == null ? null : GetSearchItem(t);
        }

        public abstract HashSet<UniversalSearchItem> GetSearchItems(HashSet<T> entities);

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<SystemEntity> entities)
        {
            return GetSearchItems(entities.OfType<T>().ToHashSet());
        }
    }
}