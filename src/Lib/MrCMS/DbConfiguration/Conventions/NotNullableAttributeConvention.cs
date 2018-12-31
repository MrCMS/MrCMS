using System.Reflection;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace MrCMS.DbConfiguration.Conventions
{
    public class NotNullableAttributeConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            var attribute = instance.Property.MemberInfo.GetCustomAttribute<NotNullableAttribute>();
            if (attribute != null)
            {
                instance.Not.Nullable();
            }
        }
    }
}