using System;
using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Website;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using Ninject;

namespace MrCMS.Services
{
    public interface IIndexService
    {
        void InitializeAllIndices(Site site = null);
        List<MrCMSIndex> GetIndexes(Site site = null);
        void Reindex(string typeName, Site site = null);
        void Optimise(string typeName, Site site = null);
        IIndexManagerBase GetIndexManagerBase(Type indexType, Site site);
    }

    public class IndexService : IIndexService
    {
        private readonly IKernel _kernel;
        private readonly ISession _session;
        private readonly Site _site;

        public IndexService(IKernel kernel, ISession session, Site site)
        {
            _kernel = kernel;
            _session = session;
            _site = site;
        }

        public void InitializeAllIndices(Site site = null)
        {
            site = site ?? _site;
            var mrCMSIndices = GetIndexes(site);
            mrCMSIndices.ForEach(index => Reindex(index.TypeName, site));
            mrCMSIndices.ForEach(index => Optimise(index.TypeName, site));
        }

        public List<MrCMSIndex> GetIndexes(Site site = null)
        {
            site = site ?? _site;
            var mrCMSIndices = new List<MrCMSIndex>();
            var indexDefinitionTypes = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(IndexDefinition<>));
            foreach (var definitionType in indexDefinitionTypes)
            {
                var indexManagerBase = GetIndexManagerBase(definitionType, site);

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

        public IIndexManagerBase GetIndexManagerBase(Type indexType, Site site)
        {
            var indexManagerBase =
                (GetIndexManagerOverride ?? DefaultGetIndexManager())(indexType, site);
            return indexManagerBase;
        }

        public static Func<Type, Site, IIndexManagerBase> GetIndexManagerOverride = null;

        private Func<Type, Site, IIndexManagerBase> DefaultGetIndexManager()
        {
            return (indexType, site) => _kernel.Get(
                typeof(IIndexManager<,>).MakeGenericType(indexType.BaseType.GetGenericArguments()[0], indexType)) as
                                                                  IIndexManagerBase;
        }

        public void Reindex(string typeName, Site site = null)
        {
            site = site ?? _site;
            var definitionType = TypeHelper.GetTypeByName(typeName);
            var indexManagerBase = GetIndexManagerBase(definitionType, site);

            var list = _session.CreateCriteria(indexManagerBase.GetEntityType()).Add(Restrictions.Eq("Site.Id", site.Id)).List();

            var listInstance =
                Activator.CreateInstance(typeof(List<>).MakeGenericType(indexManagerBase.GetEntityType()));
            var methodExt = listInstance.GetType().GetMethodExt("Add", indexManagerBase.GetEntityType());
            foreach (var entity in list)
            {
                methodExt.Invoke(listInstance, new object[] { entity });
            }
            var concreteManagerType = typeof(IIndexManager<,>).MakeGenericType(indexManagerBase.GetEntityType(), indexManagerBase.GetIndexDefinitionType());
            var methodInfo = concreteManagerType.GetMethodExt("ReIndex",
                                                              typeof(IEnumerable<>).MakeGenericType(
                                                                  indexManagerBase.GetEntityType()));

            methodInfo.Invoke(indexManagerBase, new object[] { listInstance });
        }

        public void Optimise(string typeName, Site site = null)
        {
            site = site ?? _site;
            var definitionType = TypeHelper.GetTypeByName(typeName);
            var indexManagerBase = GetIndexManagerBase(definitionType, site);

            indexManagerBase.Optimise();
        }

    }

    public class MrCMSIndex
    {
        public string Name { get; set; }
        public bool DoesIndexExist { get; set; }
        public DateTime? LastModified { get; set; }
        public int? NumberOfDocs { get; set; }
        public string TypeName { get; set; }
    }
}