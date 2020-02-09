using System.Linq;
using System.Reflection;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.MappingModel;

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
            instance.Key.ForeignKey(string.Format("FK_{0}_{1}", instance.ChildType.Name, instance.EntityType.Name));
            instance.Where("(IsDeleted = 'False' or IsDeleted = 0 or IsDeleted is null)");
        }
    }
}