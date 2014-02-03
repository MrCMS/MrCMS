using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing;
using MrCMS.Indexing.Management;
using MrCMS.Services;
using NHibernate;
using Ninject;

namespace MrCMS.Tasks
{
    internal abstract class IndexManagementTask<T> : AdHocTask, ILuceneIndexTask where T : SiteEntity
    {
        private readonly ISession _session;
        private readonly IKernel _kernel;

        protected IndexManagementTask(ISession session, IKernel kernel)
        {
            _session = session;
            _kernel = kernel;
        }

        public override int Priority
        {
            get { return 10; }
        }

        public int Id { get; set; }

        public override string GetData()
        {
            return Id.ToString();
        }

        public override void SetData(string data)
        {
            Id = int.Parse(data);
        }

        protected override void OnExecute()
        {
            List<Type> definitionTypes = IndexingHelper.GetDefinitionTypes<T>();
            var entity = GetObject();
            foreach (
                IIndexManagerBase indexManagerBase in
                    definitionTypes.Select(type => IndexService.GetIndexManagerBase(type, Site)))
                ExecuteLogic(indexManagerBase, entity);
            List<Type> relatedDefinitionTypes = IndexingHelper.GetRelatedDefinitionTypes<T>();
            foreach (Type type in relatedDefinitionTypes)
            {
                IIndexManagerBase indexManagerBase = IndexService.GetIndexManagerBase(type, Site);
                object instance = Activator.CreateInstance(type);
                MethodInfo methodInfo = type.GetMethodExt("GetEntitiesToUpdate", typeof(T));
                var toUpdate = methodInfo.Invoke(instance, new[] { entity }) as IEnumerable;
                foreach (object o in toUpdate)
                {
                    indexManagerBase.Update(o);
                }
            }
        }

        protected virtual T GetObject()
        {
            return _session.Get(typeof(T), Id) as T;
        }

        protected abstract void ExecuteLogic(IIndexManagerBase manager, T entity);
        public IEnumerable<LuceneAction> GetActions()
        {
            var luceneActions = new List<LuceneAction>();
            var entity = GetObject();
            foreach (var definitionType in IndexingHelper.GetDefinitionTypes<T>())
            {
                luceneActions.Add(new LuceneAction
                                      {
                                          Operation = Operation,
                                          Entity = entity,
                                          IndexDefinition = _kernel.Get(definitionType) as IIndexDefinition
                                      });
            }
            foreach (var relatedDefinitionType in IndexingHelper.GetRelatedDefinitionTypes<T>())
            {
                object instance = _kernel.Get(relatedDefinitionType);
                MethodInfo methodInfo = relatedDefinitionType.GetMethodExt("GetEntitiesToUpdate", typeof(T));
                var toUpdate = methodInfo.Invoke(instance, new[] { entity }) as IEnumerable;

                foreach (object o in toUpdate)
                {
                    luceneActions.Add(new LuceneAction
                    {
                        Operation = LuceneOperation.Update,
                        Entity = o as SystemEntity,
                        IndexDefinition = instance as IIndexDefinition
                    });
                }
            }

            return luceneActions;
        }

        protected abstract LuceneOperation Operation { get; }
    }

    internal interface ILuceneIndexTask
    {
        IEnumerable<LuceneAction> GetActions();
    }


    public class LuceneAction
    {
        public LuceneOperation Operation { get; set; }

        public IIndexDefinition IndexDefinition { get; set; }
        public SystemEntity Entity { get; set; }

        public Type Type
        {
            get { return IndexDefinition.GetType(); }
        }

        public void Execute(IndexWriter writer)
        {
            var document = IndexDefinition.Convert(Entity);
            var index = IndexDefinition.GetIndex(Entity);
            if (document == null || index == null)
                return;
            switch (Operation)
            {
                case LuceneOperation.Insert:
                    writer.AddDocument(document);
                    break;
                case LuceneOperation.Update:
                    writer.UpdateDocument(index, document);
                    break;
                case LuceneOperation.Delete:
                    writer.DeleteDocuments(index);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    public enum LuceneOperation
    {
        Insert,
        Update,
        Delete
    }
}