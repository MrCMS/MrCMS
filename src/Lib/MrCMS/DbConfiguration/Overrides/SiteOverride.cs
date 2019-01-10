using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Multisite;

namespace MrCMS.DbConfiguration.Overrides
{
    public class SiteOverride : IAutoMappingOverride<Site>
    {
        public void Override(AutoMapping<Site> mapping)
        {
            mapping.Map(x => x.BaseUrl).Index("idx_Site_BaseUrl");
        }
    }
}