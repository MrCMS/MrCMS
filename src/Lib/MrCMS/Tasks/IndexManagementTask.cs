using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Services;

namespace MrCMS.Tasks
{
    internal abstract class IndexManagementTask<T> : AdHocTask, ILuceneIndexTask where T : SiteEntity
    {
        private readonly IDataReader _dataReader;
        private readonly IIndexService _indexService;
        private readonly IServiceProvider _serviceProvider;
        //private readonly ISession _session;

        protected IndexManagementTask(IDataReader dataReader, IIndexService indexService, IServiceProvider serviceProvider)
        {
            _dataReader = dataReader;
            _indexService = indexService;
            _serviceProvider = serviceProvider;
        }

        public override int Priority
        {
            get { return 10; }
        }

        public int Id { get; set; }
        protected abstract LuceneOperation Operation { get; }

        public async Task<IEnumerable<LuceneAction>> GetActions(CancellationToken token)
        {
            T entity = await GetObject(token);
            return TypeHelper.GetAllConcreteTypesAssignableFrom<IndexDefinition>()
                .SelectMany(
                    definitionType =>
                        (_serviceProvider.GetService(definitionType) as IndexDefinition)?.GetAllActions(entity,
                            Operation) ?? Enumerable.Empty<LuceneAction>());
        }

        public override string GetData()
        {
            return Id.ToString();
        }

        public override void SetData(string data)
        {
            Id = int.Parse(data);
        }

        protected override async Task OnExecute(CancellationToken token)
        {
            var luceneActions = await GetActions(token);

            LuceneActionExecutor.PerformActions(_indexService, luceneActions.ToList());
        }

        protected virtual async Task<T> GetObject(CancellationToken token)
        {
            return await _dataReader.Get<T>(Id,token);// _session.Get(typeof(T), Id) as T;;
        }

        protected abstract void ExecuteLogic(IIndexManagerBase manager, T entity);
    }
}