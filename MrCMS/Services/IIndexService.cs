using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Website;
using System.Linq;
using NHibernate;

namespace MrCMS.Services
{
    public interface IIndexService
    {
        List<MrCMSIndex> GetIndexes();
        void Reindex(string typeName);
        void Optimise(string typeName);
    }

    public class IndexService : IIndexService
    {
        private readonly ISession _session;

        public IndexService(ISession session)
        {
            _session = session;
        }

        public List<MrCMSIndex> GetIndexes()
        {
            var mrCMSIndices = new List<MrCMSIndex>();
            var indexDefinitionTypes = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(IIndexDefinition<>));
            foreach (var definitionType in indexDefinitionTypes)
            {
                var indexManagerBase = GetIndexManagerBase(definitionType);

                if (indexManagerBase != null)
                {
                    mrCMSIndices.Add(new MrCMSIndex
                                         {
                                             Name = indexManagerBase.IndexName,
                                             DoesIndexExist = indexManagerBase.IndexExists,
                                             LastModified = indexManagerBase.LastModified,
                                             NumberOfDocs = indexManagerBase.NumberOfDocs,
                                             TypeName =indexManagerBase.GetIndexDefinitionType() .FullName
                                         });
                }
            }
            return mrCMSIndices;
        }

        public static IIndexManagerBase GetIndexManagerBase(Type indexType)
        {
            var indexDefinitionInterface =
                indexType.GetInterfaces()
                         .FirstOrDefault(
                             type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof (IIndexDefinition<>));
            ;
            var indexManagerBase =
                MrCMSApplication.Get(
                    typeof (FSDirectoryIndexManager<,>).MakeGenericType(indexDefinitionInterface.GetGenericArguments()[0],
                                                                        indexType)) as IIndexManagerBase;
            return indexManagerBase;
        }

        public void Reindex(string typeName)
        {
            var definitionType = TypeHelper.GetTypeByName(typeName);
            var indexManagerBase = GetIndexManagerBase(definitionType);

            var list = _session.CreateCriteria(indexManagerBase.GetEntityType()).List();

            var listInstance =
                Activator.CreateInstance(typeof(List<>).MakeGenericType(indexManagerBase.GetEntityType()));
            var methodExt = listInstance.GetType().GetMethodExt("Add", indexManagerBase.GetEntityType());
            foreach (var entity in list)
            {
                methodExt.Invoke(listInstance, new object[] {entity});
            }


            var concreteManagerType = typeof (IIndexManager<,>).MakeGenericType(indexManagerBase.GetEntityType(), indexManagerBase.GetIndexDefinitionType());
            var methodInfo = concreteManagerType.GetMethodExt("ReIndex",
                                                              typeof (IEnumerable<>).MakeGenericType(
                                                                  indexManagerBase.GetEntityType()));

            methodInfo.Invoke(indexManagerBase, new object[] { listInstance });
        }

        public void Optimise(string typeName)
        {
            var definitionType = TypeHelper.GetTypeByName(typeName);
            var indexManagerBase = GetIndexManagerBase(definitionType);

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