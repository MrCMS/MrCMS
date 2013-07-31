using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace MrCMS.DbConfiguration.Conventions
{
    public class ReferencesConvention : IReferenceConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.Cascade.SaveUpdate();
            instance.NotFound.Ignore();

            instance.ForeignKey(string.Format("FK_{0}_{1}",
                (instance.EntityType != null) ? instance.EntityType.Name : instance.Name, instance.Property.Name));


            instance.Index(string.Format("IX_{0}_{1}", instance.EntityType.Name, instance.Property.Name));
        
        }
    }
}