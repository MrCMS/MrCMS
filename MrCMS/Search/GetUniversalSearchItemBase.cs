using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Search.Models;

namespace MrCMS.Search
{
    public abstract class GetUniversalSearchItemBase
    {
        public abstract UniversalSearchItem GetSearchItem(SystemEntity entity);
    }

    public abstract class GetUniversalSearchItemBase<T> : GetUniversalSearchItemBase where T : SystemEntity
    {
        public abstract UniversalSearchItem GetSearchItem(T mediaCategory);

        public override UniversalSearchItem GetSearchItem(SystemEntity entity)
        {
            var t = entity as T;
            return t == null ? null : GetSearchItem(t);
        }
    }
}