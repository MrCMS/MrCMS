using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public abstract class IndexManagementTask : BackgroundTask
    {
        protected SiteEntity Entity;

        protected IndexManagementTask(SiteEntity entity)
        {
            Entity = entity;
        }
        protected List<Type> GetDefinitionTypes(Type entityType)
        {
            var indexDefinitionTypes = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(IIndexDefinition<>));
            var definitionTypes = indexDefinitionTypes.Where(type =>
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
                                                                         genericArgument.IsAssignableFrom(entityType);
                                                                 }).ToList();
            return definitionTypes;
        }

        public override void Execute()
        {
            if (Entity != null)
            {
                Entity = GetObject();
                CurrentRequestData.SetTaskSite(Session.Get<Site>(Entity.Site.Id));
                ExecuteLogic();
                CurrentRequestData.SetTaskSite(null);
            }
        }

        protected virtual SiteEntity GetObject()
        {
            return Session.Get(Entity.GetType(), Entity.Id) as SiteEntity;
        }

        protected abstract void ExecuteLogic();
    }
}