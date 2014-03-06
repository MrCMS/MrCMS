using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private readonly IIndexService _indexService;

        protected IndexManagementTask(ISession session, IKernel kernel, IIndexService indexService)
        {
            _session = session;
            _kernel = kernel;
            _indexService = indexService;
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
            var luceneActions = GetActions().ToList();

            LuceneActionExecutor.PerformActions(_indexService, luceneActions);
        }

        protected virtual T GetObject()
        {
            return _session.Get(typeof(T), Id) as T;
        }

        protected abstract void ExecuteLogic(IIndexManagerBase manager, T entity);
        public IEnumerable<LuceneAction> GetActions()
        {
            var entity = GetObject();
            return
                IndexingHelper.IndexDefinitionTypes.SelectMany(
                    definitionType => definitionType.GetAllActions(entity, Operation));
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

        public IndexDefinition IndexDefinition { get; set; }
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