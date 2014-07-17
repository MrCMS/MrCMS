using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Indexing;
using MrCMS.Indexing.Management;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Tasks
{
    internal abstract class IndexManagementTask<T> : AdHocTask, ILuceneIndexTask where T : SiteEntity
    {
        private readonly IIndexService _indexService;
        private readonly ISession _session;

        protected IndexManagementTask(ISession session, IIndexService indexService)
        {
            _session = session;
            _indexService = indexService;
        }

        public override int Priority
        {
            get { return 10; }
        }

        public int Id { get; set; }
        protected abstract LuceneOperation Operation { get; }

        public IEnumerable<LuceneAction> GetActions()
        {
            T entity = GetObject();
            return
                IndexingHelper.IndexDefinitionTypes.SelectMany(
                    definitionType => definitionType.GetAllActions(entity, Operation));
        }

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
            List<LuceneAction> luceneActions = GetActions().ToList();

            LuceneActionExecutor.PerformActions(_indexService, luceneActions);
        }

        protected virtual T GetObject()
        {
            return _session.Get(typeof (T), Id) as T;
        }

        protected abstract void ExecuteLogic(IIndexManagerBase manager, T entity);
    }
}