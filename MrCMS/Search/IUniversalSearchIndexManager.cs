using System;
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
        void ReindexAll();
        void Optimise();
        IndexSearcher GetSearcher();
        void Write(Action<IndexWriter> writeFunc, bool recreateIndex = false);
        
        void EnsureIndexExists();
        MrCMSIndex GetUniversalIndexInfo();
    }
}