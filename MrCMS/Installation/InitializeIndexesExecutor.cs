using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Search;
using MrCMS.Website;
using Ninject;

namespace MrCMS.Installation
{
    public class InitializeIndexesExecutor : ExecuteEndRequestBase<InitializeIndexes, int>
    {
        private readonly IKernel _kernel;
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public InitializeIndexesExecutor(IKernel kernel, IUniversalSearchIndexManager universalSearchIndexManager)
        {
            _kernel = kernel;
            _universalSearchIndexManager = universalSearchIndexManager;
        }

        public override void Execute(IEnumerable<int> data)
        {
            var indexDefinitionTypes = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(IndexDefinition<>));
            foreach (Type definitionType in indexDefinitionTypes)
            {
                IIndexManagerBase indexManagerBase = GetIndexManagerBase(definitionType);

                indexManagerBase.ReIndex();
            }
            _universalSearchIndexManager.ReindexAll();
        }

        private IIndexManagerBase GetIndexManagerBase(Type indexType)
        {
            return _kernel.Get(
                typeof(IIndexManager<,>).MakeGenericType(indexType.BaseType.GetGenericArguments()[0], indexType)) as
                IIndexManagerBase;
        }
    }
}