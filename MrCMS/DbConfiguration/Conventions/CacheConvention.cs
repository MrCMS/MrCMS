using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace MrCMS.DbConfiguration.Conventions
{
    public class CacheConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            instance.Cache.ReadWrite();
        }
    }
}