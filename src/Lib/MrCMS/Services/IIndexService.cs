using System;
using System.Collections.Generic;
using MrCMS.Indexing.Management;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IIndexService
    {
        void InitializeAllIndices();
        List<MrCMSIndex> GetIndexes();
        void Reindex(string typeName);
        IIndexManagerBase GetIndexManagerBase(Type indexType);
        IEnumerable<IIndexManagerBase> GetAllIndexManagers();
    }
}