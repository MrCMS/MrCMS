using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace MrCMS.DbConfiguration.Conventions
{
    public class AddIndexConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            if (instance.Name == "DocumentType")
            {
                instance.Index(string.Format("IX_{0}_{1}", instance.EntityType.Name, instance.Property.Name));
            }

            if (instance.Name == "IsDeleted")
            {
                instance.Index(string.Format("IX_{0}_{1}", instance.EntityType.Name, instance.Property.Name));
            }

            if (instance.Name == "UrlSegment")
            {
                instance.Index(string.Format("IX_{0}_{1}", instance.EntityType.Name, instance.Property.Name));
            }

            if (instance.Name == "BaseUrl")
            {
                instance.Index(string.Format("IX_{0}_{1}", instance.EntityType.Name, instance.Property.Name));
            }

            if (instance.Name == "ParentId")
            {
                instance.Index(string.Format("IX_{0}_{1}", instance.EntityType.Name, instance.Property.Name));
            }

            if (instance.Name == "PublishOn")
            {
                instance.Index(string.Format("IX_{0}_{1}", instance.EntityType.Name, instance.Property.Name));
            }

            if (instance.Name == "DisplayOrder")
            {
                instance.Index(string.Format("IX_{0}_{1}", instance.EntityType.Name, instance.Property.Name));
            }

            if (instance.Name == "SiteId")
            {
                instance.Index(string.Format("IX_{0}_{1}", instance.EntityType.Name, instance.Property.Name));
            }
        }
    }
}