using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Models;
using NHibernate;
using Ninject;

namespace MrCMS.Services
{
    public class IndexService : IIndexService
    {
        private readonly IKernel _kernel;

        public IndexService(IKernel kernel, IStatelessSession session, Site site)
        {
            _kernel = kernel;
        }

        public void InitializeAllIndices()
        {
            List<MrCMSIndex> mrCMSIndices = GetIndexes();
            mrCMSIndices.ForEach(index => Reindex(index.TypeName));
            mrCMSIndices.ForEach(index => Optimise(index.TypeName));
        }

        public IEnumerable<IIndexManagerBase> GetAllIndexManagers()
        {
            var indexDefinitionTypes = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof (IndexDefinition<>));
            return indexDefinitionTypes.Select(GetIndexManagerBase);
        }
        public List<MrCMSIndex> GetIndexes()
        {
            return GetAllIndexManagers().Select(indexManagerBase => new MrCMSIndex
            {
                Name = indexManagerBase.IndexName,
                DoesIndexExist = indexManagerBase.IndexExists,
                LastModified = indexManagerBase.LastModified,
                NumberOfDocs = indexManagerBase.NumberOfDocs,
                TypeName = indexManagerBase.GetIndexDefinitionType().FullName
            }).ToList();
        }

        public static Func<Type, IIndexManagerBase> GetIndexManagerOverride = null;

        public IIndexManagerBase GetIndexManagerBase(Type indexType)
        {
            IIndexManagerBase indexManagerBase =
                (GetIndexManagerOverride ?? DefaultGetIndexManager())(indexType);
            return indexManagerBase;
        }

        public void Reindex(string typeName)
        {
            Type definitionType = TypeHelper.GetTypeByName(typeName);
            IIndexManagerBase indexManagerBase = GetIndexManagerBase(definitionType);

            indexManagerBase.ReIndex();
        }

        public void Optimise(string typeName)
        {
            Type definitionType = TypeHelper.GetTypeByName(typeName);
            IIndexManagerBase indexManagerBase = GetIndexManagerBase(definitionType);

            indexManagerBase.Optimise();
        }

        private Func<Type, IIndexManagerBase> DefaultGetIndexManager()
        {
            return indexType => _kernel.Get(
                typeof (IIndexManager<,>).MakeGenericType(indexType.BaseType.GetGenericArguments()[0], indexType)) as
                IIndexManagerBase;
        }
    }
}