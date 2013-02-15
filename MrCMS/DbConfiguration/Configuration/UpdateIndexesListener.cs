using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Services;
using NHibernate.Event;
using System.Linq;

namespace MrCMS.DbConfiguration.Configuration
{
    public class UpdateIndexesListener : IPostUpdateEventListener, IPostInsertEventListener, IPostDeleteEventListener
    {
        private static List<Type> GetDefinitionTypes(Type entityType)
        {
            var indexDefinitionTypes = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof (IIndexDefinition<>));
            var definitionTypes = indexDefinitionTypes.Where(type =>
                                                                 {
                                                                     var indexDefinitionInterface =
                                                                         type.GetInterfaces()
                                                                             .FirstOrDefault(
                                                                                 interfaceType =>
                                                                                 interfaceType.IsGenericType &&
                                                                                 interfaceType.GetGenericTypeDefinition() ==
                                                                                 typeof (IIndexDefinition<>));
                                                                     var genericArgument =
                                                                         indexDefinitionInterface.GetGenericArguments()[
                                                                             0];

                                                                     return
                                                                         genericArgument.IsAssignableFrom(entityType);
                                                                 }).ToList();
            return definitionTypes;
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            var definitionTypes = GetDefinitionTypes(@event.Entity.GetType());
            foreach (var indexManagerBase in definitionTypes.Select(IndexService.GetIndexManagerBase))
                indexManagerBase.Update(@event.Entity);
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            var definitionTypes = GetDefinitionTypes(@event.Entity.GetType());
            foreach (var indexManagerBase in definitionTypes.Select(IndexService.GetIndexManagerBase))
                indexManagerBase.Insert(@event.Entity);
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            var definitionTypes = GetDefinitionTypes(@event.Entity.GetType());
            foreach (var indexManagerBase in definitionTypes.Select(IndexService.GetIndexManagerBase))
                indexManagerBase.Delete(@event.Entity);
        }
    }
}