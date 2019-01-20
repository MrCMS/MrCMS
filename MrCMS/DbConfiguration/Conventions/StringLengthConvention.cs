using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.DbConfiguration.Types;

namespace MrCMS.DbConfiguration.Conventions
{
    public class StringLengthConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            if (instance.Property.PropertyType != typeof(string))
                return;
            var memberInfo = instance.Property.MemberInfo;
            var stringLengthAttribute = memberInfo.GetCustomAttribute<StringLengthAttribute>();
            var maxLengthAttribute = memberInfo.GetCustomAttribute<MaxLengthAttribute>();
            var isDbLengthAttribute = memberInfo.GetCustomAttribute<IsDBLengthAttribute>();
            if ((stringLengthAttribute != null || maxLengthAttribute != null) && isDbLengthAttribute != null)
            {
                instance.Length(stringLengthAttribute?.MaximumLength ?? maxLengthAttribute.Length);
            }
            else
            {
                instance.CustomType<VarcharMax>();
                instance.Length(4001);
            }
        }
    }
}