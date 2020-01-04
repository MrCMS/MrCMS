using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Models;
using MrCMS.Search;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models;


namespace MrCMS.Web.Apps.Admin.Services
{
    public class IndexAdminService : IIndexAdminService
    {
        private readonly IIndexService _indexService;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISession _session;
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public IndexAdminService(IServiceProvider serviceProvider, ISession session,
            IUniversalSearchIndexManager universalSearchIndexManager,
            IIndexService indexService, IMapper mapper)
        {
            _serviceProvider = serviceProvider;
            _session = session;
            _universalSearchIndexManager = universalSearchIndexManager;
            _indexService = indexService;
            _mapper = mapper;
        }

        public List<UpdateLuceneFieldBoostModel> GetBoosts(string type)
        {
            Type definitionType = TypeHelper.GetTypeByName(type);

            if (_serviceProvider.GetService(definitionType) is IndexDefinition indexDefinition)
            {
                var luceneFieldBoosts = indexDefinition.DefinitionInfos.Select(info => info.TypeName)
                    .Select(
                        fieldName =>
                            _session.QueryOver<LuceneFieldBoost>()
                                .Where(boost => boost.Definition == fieldName)
                                .Cacheable()
                                .SingleOrDefault() ?? new LuceneFieldBoost
                            {
                                Definition = fieldName,
                            }).ToList();
                return _mapper.Map<List<UpdateLuceneFieldBoostModel>>(luceneFieldBoosts);
            }

            return new List<UpdateLuceneFieldBoostModel>();
        }

        public void SaveBoosts(List<UpdateLuceneFieldBoostModel> boosts)
        {
            _session.Transact(session => boosts.ForEach(model =>
            {
                if (model.Id == 0)
                {
                    session.Save(_mapper.Map<LuceneFieldBoost>(model));
                }
                else
                {
                    var luceneFieldBoost = session.Get<LuceneFieldBoost>(model.Id);
                    _mapper.Map(model, luceneFieldBoost);
                    session.Update(luceneFieldBoost);
                }
            }));
        }

        public MrCMSIndex GetUniversalSearchIndexInfo()
        {
            return _universalSearchIndexManager.GetUniversalIndexInfo();
        }

        public void ReindexUniversalSearch()
        {
            _universalSearchIndexManager.ReindexAll();
        }


        public List<MrCMSIndex> GetIndexes()
        {
            return _indexService.GetIndexes();
        }

        public void Reindex(string typeName)
        {
            _indexService.Reindex(typeName);
        }
    }
}