using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Search.Models;
using NHibernate;
using NHibernate.Criterion;
using Ninject;
using StackExchange.Profiling;

namespace MrCMS.Search
{
    public class UniversalSearchItemGenerator : IUniversalSearchItemGenerator
    {
        public static readonly Dictionary<Type, Type> GetUniversalSearchItemTypes;
        private readonly IKernel _kernel;
        private readonly ISearchConverter _searchConverter;
        private readonly IStatelessSession _session;

        static UniversalSearchItemGenerator()
        {
            GetUniversalSearchItemTypes = new Dictionary<Type, Type>();

            foreach (
                Type type in
                    TypeHelper.GetAllConcreteMappedClassesAssignableFrom<SystemEntity>()
                        .Where(type => !type.ContainsGenericParameters))
            {
                Type thisType = type;
                while (typeof(SystemEntity).IsAssignableFrom(thisType))
                {
                    Type itemGeneratorType =
                        TypeHelper.GetAllConcreteTypesAssignableFrom(
                            typeof(GetUniversalSearchItemBase<>).MakeGenericType(thisType)).FirstOrDefault();
                    if (itemGeneratorType != null)
                    {
                        GetUniversalSearchItemTypes.Add(type, itemGeneratorType);
                        break;
                    }
                    thisType = thisType.BaseType;
                }
            }
        }

        public UniversalSearchItemGenerator(IKernel kernel, ISearchConverter searchConverter, IStatelessSession session)
        {
            _kernel = kernel;
            _searchConverter = searchConverter;
            _session = session;
        }

        public bool CanGenerate(SystemEntity entity)
        {
            return entity != null && GetUniversalSearchItemTypes.ContainsKey(entity.GetType());
        }

        public Document GenerateDocument(SystemEntity entity)
        {
            if (entity == null)
                return null;

            Type type = entity.GetType();
            if (!GetUniversalSearchItemTypes.ContainsKey(type))
                return null;

            var getUniversalSearchItem = _kernel.Get(GetUniversalSearchItemTypes[type]) as GetUniversalSearchItemBase;
            if (getUniversalSearchItem == null)
                return null;

            UniversalSearchItem item = getUniversalSearchItem.GetSearchItem(entity);
            if (item == null)
                return null;
            return _searchConverter.Convert(item);
        }

        public IEnumerable<Document> GetAllItems()
        {
            foreach (Type universalSearchItemType in GetUniversalSearchItemTypes.Keys.OrderByDescending(type => type.FullName))
            {
                using (MiniProfiler.Current.Step("Getting " + universalSearchItemType.FullName))
                {
                    var objects = new HashSet<object>();
                    using (MiniProfiler.Current.Step("Loading objects " + universalSearchItemType.Name))
                    {
                        Type type = universalSearchItemType;
                        objects.AddRange(
                            _session.CreateCriteria(universalSearchItemType)
                                .SetCacheable(true)
                                .List()
                                .Cast<object>()
                                .Where(o => o.GetType() == type));
                    }
                    foreach (
                        var generateDocument in
                            GenerateDocuments(objects.OfType<SystemEntity>().ToHashSet(), universalSearchItemType))
                    {
                        yield return generateDocument;
                    }
                }

            }
        }

        private HashSet<Document> GenerateDocuments(HashSet<SystemEntity> entities, Type type)
        {
            using (MiniProfiler.Current.Step("Generating documents for " + type.Name))
            {
                if (entities == null || !entities.Any())
                    return new HashSet<Document>();

                if (!GetUniversalSearchItemTypes.ContainsKey(type))
                    return new HashSet<Document>();

                var getUniversalSearchItem =
                    _kernel.Get(GetUniversalSearchItemTypes[type]) as GetUniversalSearchItemBase;
                if (getUniversalSearchItem == null)
                    return new HashSet<Document>();

                HashSet<UniversalSearchItem> searchItems = getUniversalSearchItem.GetSearchItems(entities);
                if (!searchItems.Any())
                    return new HashSet<Document>();
                using (MiniProfiler.Current.Step("Convert items for " + type.Name))
                {
                    return searchItems.Select(item => _searchConverter.Convert(item)).ToHashSet();
                }
            }
        }
    }
}