using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using MrCMS.DbConfiguration.Types;

namespace MrCMS.DbConfiguration.Conventions
{
    public class DateTimeConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            if (instance.Property.PropertyType == typeof(DateTime))
                instance.CustomType<DateTimeData>();
            else if (instance.Property.PropertyType == typeof(DateTime?))
                instance.CustomType<NullableDateTimeData>();
        }
    }
}