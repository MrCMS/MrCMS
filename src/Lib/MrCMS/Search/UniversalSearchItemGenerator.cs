using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Lucene.Net.Documents;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Search.Models;

namespace MrCMS.Search
{
    public class UniversalSearchItemGenerator : IUniversalSearchItemGenerator
    {
        public static readonly Dictionary<Type, Type> GetUniversalSearchItemTypes;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISearchConverter _searchConverter;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly Site _site;

        static UniversalSearchItemGenerator()
        {
            GetUniversalSearchItemTypes = new Dictionary<Type, Type>();

            foreach (
                var type in
                TypeHelper.GetAllConcreteMappedClassesAssignableFrom<SystemEntity>()
                    .Where(type => !type.ContainsGenericParameters))
            {
                var thisType = type;
                while (typeof(SystemEntity).IsAssignableFrom(thisType))
                {
                    var itemGeneratorType =
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

        public UniversalSearchItemGenerator(IServiceProvider serviceProvider, ISearchConverter searchConverter,
            IRepositoryResolver repositoryResolver,
            Site site)
        {
            _serviceProvider = serviceProvider;
            _searchConverter = searchConverter;
            _repositoryResolver = repositoryResolver;
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

            var type = entity.GetType();
            if (!GetUniversalSearchItemTypes.ContainsKey(type))
                return null;

            var getUniversalSearchItem = _serviceProvider.GetService(GetUniversalSearchItemTypes[type]) as GetUniversalSearchItemBase;
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
                {
                    items = new Dictionary<int, UniversalSearchItem>();
                }
                else
                {
                    var getUniversalSearchItem =
                        _serviceProvider.GetService(GetUniversalSearchItemTypes[key]) as GetUniversalSearchItemBase;
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

        public async Task<IEnumerable<Document>> GetAllItems()
        {
            var siteMethod = TypeHelper.GetMethodExt(typeof(UniversalSearchItemGenerator), nameof(LoadAllOfType));
            var globalMethod = TypeHelper.GetMethodExt(typeof(UniversalSearchItemGenerator), nameof(GlobalLoadAllOfType));
            var docs = new List<Document>();
            foreach (var universalSearchItemType in GetUniversalSearchItemTypes.Keys.OrderByDescending(type =>
                type.FullName))
            //using (MiniProfiler.Current.Step("Getting " + universalSearchItemType.FullName))
            {
                var objects = new HashSet<object>();
                //using (MiniProfiler.Current.Step("Loading objects " + universalSearchItemType.Name))
                {
                    var type = universalSearchItemType;
                    if (typeof(SiteEntity).IsAssignableFrom(type))
                        objects.AddRange(
                            (await (Task<IEnumerable>)siteMethod.MakeGenericMethod(universalSearchItemType).Invoke(this, null))
                            .Cast<object>()
                        );
                    else
                        objects.AddRange(
                            (await (Task<IEnumerable>)globalMethod.MakeGenericMethod(universalSearchItemType).Invoke(this, null))
                            .Cast<object>()
                        );
                }

                docs.AddRange(GenerateDocuments(objects.OfType<SystemEntity>().ToHashSet(), universalSearchItemType));
            }

            return docs;
        }

        private async Task<IEnumerable<T>> LoadAllOfType<T>() where T : class, IHaveId, IHaveSite
        {
            return await _repositoryResolver.GetRepository<T>().Readonly().ToListAsync();
        }
        private async Task<IEnumerable<T>> GlobalLoadAllOfType<T>() where T : class, IHaveId
        {
            return await _repositoryResolver.GetGlobalRepository<T>().Readonly().ToListAsync();
        }

        private HashSet<Document> GenerateDocuments(HashSet<SystemEntity> entities, Type type)
        {
            //using (MiniProfiler.Current.Step("Generating documents for " + type.Name))
            {
                if (entities == null || !entities.Any())
                    return new HashSet<Document>();

                if (!GetUniversalSearchItemTypes.ContainsKey(type))
                    return new HashSet<Document>();

                var getUniversalSearchItem =
                    _serviceProvider.GetService(GetUniversalSearchItemTypes[type]) as GetUniversalSearchItemBase;
                if (getUniversalSearchItem == null)
                    return new HashSet<Document>();

                var searchItems = getUniversalSearchItem.GetSearchItems(entities);
                if (!searchItems.Any())
                    return new HashSet<Document>();
                //using (MiniProfiler.Current.Step("Convert items for " + type.Name))
                {
                    return searchItems.Select(item => _searchConverter.Convert(item)).ToHashSet();
                }
            }
        }
    }
}