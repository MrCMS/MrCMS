using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using MrCMS.Entities;
using MrCMS.Entities.Documents;

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