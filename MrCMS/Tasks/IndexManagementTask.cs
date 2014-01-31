using System;
using System.Collections;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing;
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

        protected override void OnExecute()
        {
            var definitionTypes = IndexingHelper.GetDefinitionTypes<T>();
            foreach (var indexManagerBase in definitionTypes.Select(type => IndexService.GetIndexManagerBase(type, Site)))
                ExecuteLogic(indexManagerBase, GetObject());
            var relatedDefinitionTypes = IndexingHelper.GetRelatedDefinitionTypes<T>();
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