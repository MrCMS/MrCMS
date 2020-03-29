using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MrCMS.Data;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Models;
using MrCMS.Search;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class IndexAdminService : IIndexAdminService
    {
        private readonly IIndexService _indexService;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRepository<LuceneFieldBoost> _repository;
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public IndexAdminService(IServiceProvider serviceProvider, IRepository<LuceneFieldBoost> repository,
            IUniversalSearchIndexManager universalSearchIndexManager,
            IIndexService indexService, IMapper mapper)
        {
            _serviceProvider = serviceProvider;
            _repository = repository;
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
                            _repository
                                .Readonly()
                                .SingleOrDefault(boost => boost.Definition == fieldName) ?? new LuceneFieldBoost
                                {
                                    Definition = fieldName,
                                }).ToList();
                return _mapper.Map<List<UpdateLuceneFieldBoostModel>>(luceneFieldBoosts);
            }

            return new List<UpdateLuceneFieldBoostModel>();
        }

        public async Task SaveBoosts(List<UpdateLuceneFieldBoostModel> boosts)
        {
            await _repository.Transact(async (repo, ct) =>
                {
                    foreach (var model in boosts)
                        if (model.Id == 0)
                        {
                            await repo.Add(_mapper.Map<LuceneFieldBoost>(model), ct);
                        }
                        else
                        {
                            var luceneFieldBoost = await repo.Load(model.Id, ct);
                            _mapper.Map(model, luceneFieldBoost);
                            await repo.Update(luceneFieldBoost, ct);
                        }
                }
            );
        }

        public MrCMSIndex GetUniversalSearchIndexInfo()
        {
            return _universalSearchIndexManager.GetUniversalIndexInfo();
        }

        public async Task ReindexUniversalSearch()
        {
         await   _universalSearchIndexManager.ReindexAll();
        }


        public List<MrCMSIndex> GetIndexes()
        {
            return _indexService.GetIndexes();
        }

        public async Task Reindex(string typeName)
        {
            await _indexService.Reindex(typeName);
        }
    }
}