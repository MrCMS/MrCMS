using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Search;
using MrCMS.Services;
using MrCMS.Website;
using Ninject;

namespace MrCMS.Installation
{
    public class InitializeIndexesExecutor : ExecuteEndRequestBase<InitializeIndexes, int>
    {
        private readonly IIndexService _indexService;
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public InitializeIndexesExecutor(IIndexService indexService, IUniversalSearchIndexManager universalSearchIndexManager)
        {
            _indexService = indexService;
            _universalSearchIndexManager = universalSearchIndexManager;
        }

        public override void Execute(IEnumerable<int> data)
        {
            foreach (var indexManager in _indexService.GetAllIndexManagers())
            {
                indexManager.ReIndex();
            }
            _universalSearchIndexManager.ReindexAll();
        }
    }
}