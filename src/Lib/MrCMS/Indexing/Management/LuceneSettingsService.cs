using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;

namespace MrCMS.Indexing.Management
{
    public class LuceneSettingsService : ILuceneSettingsService
    {
        private readonly IRepository<LuceneFieldBoost> _repository;
        private readonly Site _site;
        private IList<LuceneFieldBoost> _boosts;

        public LuceneSettingsService(IRepository<LuceneFieldBoost> repository, Site site)
        {
            _repository = repository;
            _site = site;
        }

        public float GetBoost<T, T2>(IFieldDefinition<T, T2> fieldDefinition)
            where T : IndexDefinition<T2>
            where T2 : SystemEntity
        {
            var luceneFieldBoost =
                (_boosts ?? (_boosts = _repository.Readonly()
                    .Where(boost => boost.Site.Id == _site.Id)
                    .ToList()))
                    .SingleOrDefault(boost => boost.Definition == fieldDefinition.TypeName);

            return luceneFieldBoost?.Boost ?? 1f;
        }
    }

    public class LuceneFieldBoost : SiteEntity
    {
        public LuceneFieldBoost()
        {
            Boost = 1;
        }
        public virtual string Definition { get; set; }
        public virtual float Boost { get; set; }
    }
}