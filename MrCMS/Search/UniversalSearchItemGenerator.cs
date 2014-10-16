using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Search.Models;
using NHibernate;
using Ninject;

namespace MrCMS.Search
{
    public class UniversalSearchItemGenerator : IUniversalSearchItemGenerator
    {
        private readonly IKernel _kernel;
        private readonly ISearchConverter _searchConverter;
        private readonly ISession _session;

        public UniversalSearchItemGenerator(IKernel kernel, ISearchConverter searchConverter,ISession session)
        {
            _kernel = kernel;
            _searchConverter = searchConverter;
            _session = session;
        }

        static UniversalSearchItemGenerator()
        {
            GetUniversalSearchItemTypes = new Dictionary<Type, Type>();

            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<SystemEntity>().Where(type => !type.ContainsGenericParameters))
            {

                var thisType = type;
                while (typeof(SystemEntity).IsAssignableFrom(thisType))
                {
                    var itemGeneratorType =
                        TypeHelper.GetAllConcreteTypesAssignableFrom(
                            typeof (GetUniversalSearchItemBase<>).MakeGenericType(thisType)).FirstOrDefault();
                    if (itemGeneratorType != null)
                    {
                        GetUniversalSearchItemTypes.Add(type, itemGeneratorType);
                        break;
                    }
                    thisType = thisType.BaseType;
                }
            }
        }

        public static readonly Dictionary<Type, Type> GetUniversalSearchItemTypes;

        public bool CanGenerate(SystemEntity entity)
        {
            return entity != null && GetUniversalSearchItemTypes.ContainsKey(entity.GetType());
        }

        public Document GenerateDocument(SystemEntity entity)
        {
            if (entity == null)
                return null;

            var type = entity.GetType();
            if (!GetUniversalSearchItemTypes.ContainsKey(type)) 
                return null;

            var getUniversalSearchItem = _kernel.Get(GetUniversalSearchItemTypes[type]) as GetUniversalSearchItemBase;
            if (getUniversalSearchItem == null) 
                return null;

            var item = getUniversalSearchItem.GetSearchItem(entity);
            if (item == null)
                return null;
            return _searchConverter.Convert(item);
        }

        public HashSet<Document> GetAllItems()
        {
            var hashSet = new HashSet<SystemEntity>();
            foreach (var universalSearchItemType in GetUniversalSearchItemTypes.Keys)
            {
                var list = _session.CreateCriteria(universalSearchItemType).SetCacheable(true).List();
                foreach (var item in list)
                {
                    var systemEntity = item as SystemEntity;
                    if (systemEntity != null)
                        hashSet.Add(systemEntity);
                }
            }
            return hashSet.Select(GenerateDocument).ToHashSet();
        }
    }
}