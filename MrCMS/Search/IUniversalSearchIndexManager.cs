using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Search
{
    public interface IUniversalSearchIndexManager
    {
        void Index(SystemEntity entity);
        void Delete(SystemEntity entity);
        void ReindexAll();
    }
}