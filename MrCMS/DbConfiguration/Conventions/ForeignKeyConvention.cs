using System;
using FluentNHibernate;
using FluentNHibernate.Conventions;

namespace MrCMS.DbConfiguration.Conventions
{
    public class CustomForeignKeyConvention : ForeignKeyConvention
    {
        protected override string GetKeyName(Member property, Type type)
        {
            if (property == null)
                return type.Name + "Id"; // many-to-many, one-to-many, join

            return property.Name + "Id"; // many-to-one
        }
    }
}