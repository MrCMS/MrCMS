using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Testing.Values;
using Lucene.Net.Analysis;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using NHibernate;

namespace MrCMS.Indexing.Management
{
    public class LuceneSettingsService : ILuceneSettingsService
    {
        private readonly ISession _session;
        private readonly Site _site;
        private IList<LuceneFieldBoost> _boosts;

        public LuceneSettingsService(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public float GetBoost<T, T2>(IFieldDefinition<T, T2> fieldDefinition)
            where T : IndexDefinition<T2>
            where T2 : SystemEntity
        {
            var luceneFieldBoost =
                (_boosts ?? (_boosts = _session.QueryOver<LuceneFieldBoost>()
                    .Where(boost => boost.Site.Id == _site.Id)
                    .Cacheable().List()))
                    .SingleOrDefault(boost => boost.Definition == fieldDefinition.TypeName);

            return luceneFieldBoost == null
                ? 1f
                : luceneFieldBoost.Boost;
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