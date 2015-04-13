using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Models;
using MrCMS.Search;
using MrCMS.Services;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class IndexAdminService : IIndexAdminService
    {
        private readonly IIndexService _indexService;
        private readonly IKernel _kernel;
        private readonly ISession _session;
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public IndexAdminService(IKernel kernel, ISession session,
            IUniversalSearchIndexManager universalSearchIndexManager, IIndexService indexService)
        {
            _kernel = kernel;
            _session = session;
            _universalSearchIndexManager = universalSearchIndexManager;
            _indexService = indexService;
        }

        public List<LuceneFieldBoost> GetBoosts(string type)
        {
            Type definitionType = TypeHelper.GetTypeByName(type);
            var indexDefinition = _kernel.Get(definitionType) as IndexDefinition;

            if (indexDefinition != null)
                return indexDefinition.DefinitionInfos.Select(info => info.TypeName)
                    .Select(
                        fieldName =>
                            _session.QueryOver<LuceneFieldBoost>()
                                .Where(boost => boost.Definition == fieldName)
                                .Cacheable()
                                .SingleOrDefault() ?? new LuceneFieldBoost
                                {
                                    Definition = fieldName,
                                }).ToList();
            return new List<LuceneFieldBoost>();
        }

        public void SaveBoosts(List<LuceneFieldBoost> boosts)
        {
            _session.Transact(session => boosts.ForEach(session.SaveOrUpdate));
        }

        public MrCMSIndex GetUniversalSearchIndexInfo()
        {
            return _universalSearchIndexManager.GetUniversalIndexInfo();
        }

        public void ReindexUniversalSearch()
        {
            _universalSearchIndexManager.ReindexAll();
        }

        public void OptimiseUniversalSearch()
        {
            _universalSearchIndexManager.Optimise();
        }

        public List<MrCMSIndex> GetIndexes()
        {
            return _indexService.GetIndexes();
        }

        public void Reindex(string typeName)
        {
            _indexService.Reindex(typeName);
        }

        public void Optimise(string typeName)
        {
            _indexService.Optimise(typeName);
        }
    }
}