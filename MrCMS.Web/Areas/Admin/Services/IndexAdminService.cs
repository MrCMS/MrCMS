using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Search;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class IndexAdminService : IIndexAdminService
    {
        private readonly IKernel _kernel;
        private readonly ISession _session;
        private readonly Site _site;
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public IndexAdminService(IKernel kernel, ISession session, Site site, IUniversalSearchIndexManager universalSearchIndexManager)
        {
            _kernel = kernel;
            _session = session;
            _site = site;
            _universalSearchIndexManager = universalSearchIndexManager;
        }

        public List<LuceneFieldBoost> GetBoosts(string type)
        {
            var definitionType = TypeHelper.GetTypeByName(type);
            var indexDefinition = _kernel.Get(definitionType) as IndexDefinition;

            if (indexDefinition != null)
                return indexDefinition.DefinitionInfos.Select(info => info.TypeName)
                    .Select(
                        fieldName =>
                            _session.QueryOver<LuceneFieldBoost>()
                                .Where(boost => boost.Site.Id == _site.Id && boost.Definition == fieldName)
                                .Cacheable()
                                .SingleOrDefault() ?? new LuceneFieldBoost
                                                      {
                                                          Definition = fieldName,
                                                          Site = _site
                                                      }).ToList();
            return new List<LuceneFieldBoost>();
        }

        public void SaveBoosts(List<LuceneFieldBoost> boosts)
        {
            _session.Transact(session => boosts.ForEach(session.SaveOrUpdate));
        }

        public void ReindexUniversalSearch()
        {
            _universalSearchIndexManager.ReindexAll();
        }
    }
}