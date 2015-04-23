using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
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
        private readonly Site _site;

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

        public UniversalSearchItemGenerator(IKernel kernel, ISearchConverter searchConverter, IStatelessSession session,Site site)
        {
            _kernel = kernel;
            _searchConverter = searchConverter;
            _session = session;
            _site = site;
        }

        public bool CanGenerate(SystemEntity entity)
        {
            return entity != null && GetUniversalSearchItemTypes.ContainsKey(entity.GetType());
        }

        public UniversalSearchItem GenerateItem(SystemEntity entity)
        {
            if (entity == null)
                return null;

            Type type = entity.GetType();
            if (!GetUniversalSearchItemTypes.ContainsKey(type))
                return null;

            var getUniversalSearchItem = _kernel.Get(GetUniversalSearchItemTypes[type]) as GetUniversalSearchItemBase;
            if (getUniversalSearchItem == null)
                return null;

            return getUniversalSearchItem.GetSearchItem(entity);
        }

        public Document GenerateDocument(SystemEntity entity)
        {
            var item = GenerateItem(entity);
            if (item == null)
                return null;
            return _searchConverter.Convert(item);
        }

        public Dictionary<SystemEntity, Document> GenerateDocuments(IEnumerable<SystemEntity> entities)
        {
            var dictionary = entities.GroupBy(entity => entity.GetType())
                .ToDictionary(grouping => grouping.Key, grouping => grouping.ToHashSet());

            var documents = new Dictionary<SystemEntity, Document>();
            foreach (var key in dictionary.Keys)
            {
                Dictionary<int, UniversalSearchItem> items;
                if (!GetUniversalSearchItemTypes.ContainsKey(key))
                    items = new Dictionary<int, UniversalSearchItem>();
                else
                {
                    var getUniversalSearchItem = _kernel.Get(GetUniversalSearchItemTypes[key]) as GetUniversalSearchItemBase;
                    if (getUniversalSearchItem == null)
                        items = new Dictionary<int, UniversalSearchItem>();
                    else
                        items = getUniversalSearchItem.GetSearchItems(dictionary[key])
                            .ToDictionary(searchItem => searchItem.Id, searchItem => searchItem);
                }
                foreach (var entity in dictionary[key])
                {
                    var item = items.ContainsKey(entity.Id) ? items[entity.Id] : null;
                    documents.Add(entity, item == null ? null : _searchConverter.Convert(item));
                }
            }

            return documents;
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
                        var criteria = _session.CreateCriteria(universalSearchItemType);
                        if (typeof (SiteEntity).IsAssignableFrom(type))
                        {
                            criteria = criteria.Add(Restrictions.Eq("Site.Id", _site.Id));
                        }
                        objects.AddRange(
                            criteria
                    .Add(Restrictions.Eq("IsDeleted", false))
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