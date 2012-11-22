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
        }
    }
}