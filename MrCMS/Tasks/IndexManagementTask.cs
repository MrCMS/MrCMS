using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Tasks
{
    internal abstract class IndexManagementTask<T> : AdHocTask where T : SiteEntity
    {
        private readonly ISession _session;

        protected IndexManagementTask(ISession session)
        {
            _session = session;
        }
        
        public override string GetData()
        {
            return Id.ToString();
        }
        public override void SetData(string data)
        {
            Id = int.Parse(data);
        }
        public override int Priority
        {
            get { return 10; }
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

        protected override void OnExecute()
        {
            var definitionTypes = GetDefinitionTypes();
            foreach (var indexManagerBase in definitionTypes.Select(type => IndexService.GetIndexManagerBase(type, Site)))
                ExecuteLogic(indexManagerBase, GetObject());
            var relatedDefinitionTypes = GetRelatedDefinitionTypes();
            foreach (var type in relatedDefinitionTypes)
            {
                var instance = Activator.CreateInstance(type);
                var indexManagerBase = IndexService.GetIndexManagerBase(type, Site);
                var methodInfo = type.GetMethodExt("GetEntitiesToUpdate", typeof(T));
                var toUpdate = methodInfo.Invoke(instance, new[] { Entity }) as IEnumerable;
                foreach (var entity in toUpdate)
                {
                    indexManagerBase.Update(entity);
                }
            }
        }

        protected virtual T GetObject()
        {
            return _session.Get(typeof(T), Id) as T;
        }

        public int Id { get; set; }

        protected abstract void ExecuteLogic(IIndexManagerBase manager, T entity);
    }
}