using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Indexing.Management;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IIndexService
    {
        Task InitializeAllIndices();
        List<MrCMSIndex> GetIndexes();
        Task Reindex(string typeName);
        IIndexManagerBase GetIndexManagerBase(Type indexType);
        IEnumerable<IIndexManagerBase> GetAllIndexManagers();
    }
}