using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.TextSearch.Entities;

namespace MrCMS.TextSearch.Services
{
    public interface ITextSearchItemUpdater
    {
        Task Add(SystemEntity entity);
        Task Add(IEnumerable<SystemEntity> entities);
        Task Update(SystemEntity entity);
        Task Delete(SystemEntity entity);
        Task Delete(IEnumerable<TextSearchItem> toDelete);
    }
}