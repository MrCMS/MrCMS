using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Search.Models;

namespace MrCMS.Search
{
    public interface IGetUniversalIndexStatuses
    {
        Dictionary<SystemEntity, UniversalSearchIndexStatus> GetStatuses(IEnumerable<SystemEntity> entities);
    }
}