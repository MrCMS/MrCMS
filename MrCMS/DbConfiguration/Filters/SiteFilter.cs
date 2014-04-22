using FluentNHibernate.Mapping;
using NHibernate;

namespace MrCMS.DbConfiguration.Filters
{
    public class SiteFilter : FilterDefinition
    {
        public SiteFilter()
        {
            WithName("SiteFilter").WithCondition("(SiteId = :site or SiteId is null)").AddParameter("site", NHibernateUtil.Int32);
        }
    }
}