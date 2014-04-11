using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using MrCMS.DbConfiguration.Filters;
using MrCMS.Entities;

namespace MrCMS.DbConfiguration.Conventions
{
    public class SiteFilterConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            if (instance.EntityType.IsSubclassOf(typeof (SiteEntity)))
            {
                instance.ApplyFilter<SiteFilter>();
            }
        }
    }
}