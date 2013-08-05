using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public abstract class IndexManagementTask<T> : BackgroundTask where T : SiteEntity
    {
        protected T Entity;

        protected IndexManagementTask(T entity)
            : base(entity.Site)
        {
            Entity = entity;
        }
        protected List<Type> GetDefinitionTypes()
        {
            var definitionTypes = IndexDefinitionTypes.Where(type =>
            {
                var indexDefinitionInterface =
                    type.GetInterfaces()
                        .FirstOrDefault(
                            interfaceType =>
                            interfaceType.IsGenericType &&
                            interfaceType.GetGenericTypeDefinition() ==
                            typeof(IIndexDefinition<>));
                var genericArgument =
                    indexDefinitionInterface.GetGenericArguments()[
                        0];

                return
                    genericArgument.IsAssignableFrom(typeof(T));
            }).ToList();
            return definitionTypes;
        }

        protected List<Type> GetRelatedDefinitionTypes()
        {
            var definitionTypes = IndexDefinitionTypes.Where(type =>
                                                                 {
                                                                     var interfaces =
                                                                         type.GetInterfaces()
                                                                             .Where(
                                                                                 interfaceType =>
                                                                                 interfaceType.IsGenericType &&
                                                                                 interfaceType.GetGenericTypeDefinition() ==
                                                                                 typeof(IRelatedItemIndexDefinition<,>));

                                                                     return
                                                                         interfaces.Any(
                                                                             relatedDefinitionType =>
                                                                             relatedDefinitionType.GetGenericArguments()
                                                                                 [0].IsAssignableFrom(typeof(T)));

                                                                 }).ToList();
            return definitionTypes;
        }

        private static List<Type> IndexDefinitionTypes
        {
            get { return TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(IIndexDefinition<>)); }
        }

        public override void Execute()
        {
            if (Entity != null)
            {
                Entity = GetObject();
                var site = Session.Get<Site>(Entity.Site.Id);
                CurrentRequestData.CurrentSite = site;

                var definitionTypes = GetDefinitionTypes();
                foreach (var indexManagerBase in definitionTypes.Select(type => IndexService.GetIndexManagerBase(type, site)))
                    ExecuteLogic(indexManagerBase, Entity);
                var relatedDefinitionTypes = GetRelatedDefinitionTypes();
                foreach (var type in relatedDefinitionTypes)
                {
                    var instance = Activator.CreateInstance(type);
                    var indexManagerBase = IndexService.GetIndexManagerBase(type, site);
                    var methodInfo = type.GetMethodExt("GetEntitiesToUpdate", typeof(T));
                    var toUpdate = methodInfo.Invoke(instance, new[] { Entity }) as IEnumerable;
                    foreach (var entity in toUpdate)
                    {
                        indexManagerBase.Update(entity);
                    }
                }
            }
        }

        protected virtual T GetObject()
        {
            return Session.Get(typeof(T), Entity.Id) as T;
        }

        protected abstract void ExecuteLogic(IIndexManagerBase manager, T entity);
    }
}