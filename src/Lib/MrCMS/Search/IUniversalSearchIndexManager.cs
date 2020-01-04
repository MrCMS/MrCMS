using System;
using System.Threading.Tasks;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Entities;
using MrCMS.Models;

namespace MrCMS.Search
{
    public interface IUniversalSearchIndexManager
    {
        void Insert(SystemEntity entity);
        void Update(SystemEntity entity);
        void Delete(SystemEntity entity);
        Task ReindexAll();
        void Optimise();
        IndexSearcher GetSearcher();
        Task Write(Func<IndexWriter, Task> writeFunc, bool recreateIndex = false);

        void EnsureIndexExists();
        MrCMSIndex GetUniversalIndexInfo();
    }
}