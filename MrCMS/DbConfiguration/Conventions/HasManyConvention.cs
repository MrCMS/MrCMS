using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using MrCMS.DbConfiguration.Filters;

namespace MrCMS.DbConfiguration.Conventions
{
    public class HasManyConvention : IHasManyConvention
    {
        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Cache.ReadWrite();
            instance.Cascade.SaveUpdate();
            instance.Cascade.AllDeleteOrphan();
            instance.Fetch.Subselect();
            instance.Inverse();
            instance.Relationship.NotFound.Ignore();
            instance.ApplyFilter<NotDeletedFilter>();
        }
    }
}