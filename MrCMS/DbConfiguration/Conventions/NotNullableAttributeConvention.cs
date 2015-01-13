using System;
using System.Reflection;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using MrCMS.Entities;

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
    public class RootGuidConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            if (instance.Property.Name == "Guid" && instance.Property.PropertyType == typeof (Guid))
            {
                instance.Access.ReadOnlyPropertyThroughCamelCaseField(CamelCasePrefix.Underscore);
            }
        }
    }
}