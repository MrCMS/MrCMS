using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Models;
using NHibernate;
using NHibernate.Criterion;
using Ninject;
using StackExchange.Profiling;

namespace MrCMS.Services
{
    public class IndexService : IIndexService
    {
        private readonly IKernel _kernel;
        private readonly IStatelessSession _session;
        private readonly Site _site;

        public IndexService(IKernel kernel, IStatelessSession session, Site site)
        {
            _kernel = kernel;
            _session = session;
            _site = site;
        }

        public void InitializeAllIndices()
        {
            List<MrCMSIndex> mrCMSIndices = GetIndexes();
            mrCMSIndices.ForEach(index => Reindex(index.TypeName));
            mrCMSIndices.ForEach(index => Optimise(index.TypeName));
        }

        public List<MrCMSIndex> GetIndexes()
        {
            var mrCMSIndices = new List<MrCMSIndex>();
            var indexDefinitionTypes = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof (IndexDefinition<>));
            foreach (Type definitionType in indexDefinitionTypes)
            {
                IIndexManagerBase indexManagerBase = GetIndexManagerBase(definitionType);

                if (indexManagerBase != null)
                {
                    mrCMSIndices.Add(new MrCMSIndex
                    {
                        Name = indexManagerBase.IndexName,
                        DoesIndexExist = indexManagerBase.IndexExists,
                        LastModified = indexManagerBase.LastModified,
                        NumberOfDocs = indexManagerBase.NumberOfDocs,
                        TypeName = indexManagerBase.GetIndexDefinitionType().FullName
                    });
                }
            }
            return mrCMSIndices;
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


            var entityType = indexManagerBase.GetEntityType();
            IList list = GetEntities(entityType);
            Type concreteManagerType = typeof(IIndexManager<,>).MakeGenericType(entityType, indexManagerBase.GetIndexDefinitionType());
            MethodInfo methodInfo = concreteManagerType.GetMethodExt("ReIndex", typeof(IEnumerable<>).MakeGenericType(entityType));

            var listInstance = CreateList(entityType, list);

            methodInfo.Invoke(indexManagerBase, new[] { listInstance });
        }

        private static object CreateList(Type parameterTypes, IList list)
        {
            using (MiniProfiler.Current.Step("Build list"))
            {
                object listInstance =
                    Activator.CreateInstance(typeof(List<>).MakeGenericType(parameterTypes));
                MethodInfo methodExt = listInstance.GetType().GetMethodExt("Add", parameterTypes);
                foreach (object entity in list)
                {
                    methodExt.Invoke(listInstance, new[] { entity });
                }

                return listInstance;
            }
        }

        private IList GetEntities(Type entityType)
        {
            using (MiniProfiler.Current.Step("Load all entities"))
            {
                return _session.CreateCriteria(entityType)
                    .Add(Restrictions.Eq("Site.Id", _site.Id))
                    .Add(Restrictions.Eq("IsDeleted", false))
                    .SetCacheable(true)
                    .List();
            }
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