using System;
using System.Linq;
using System.Reflection;
using FluentNHibernate.Automapping;
using MrCMS.Entities;

namespace MrCMS.DbConfiguration.Mapping
{
    public class MrCMSMappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.IsSubclassOf(typeof(SystemEntity)) &&
                   base.ShouldMap(type) && !HasDoNotMapAttribute(type) && !type.Assembly.IsDynamic;
        }

        private bool HasDoNotMapAttribute(Type type)
        {
            return type.GetCustomAttribute<DoNotMapAttribute>() != null;
        }

        public override bool ShouldMap(FluentNHibernate.Member member)
        {
            return (member.CanWrite || member.MemberInfo.GetCustomAttribute<ShouldMapAttribute>() != null) && base.ShouldMap(member);
        }

        public override bool IsId(FluentNHibernate.Member member)
        {
            return member.Name == "Id" && base.IsId(member);
        }

        public override bool IsDiscriminated(System.Type type)
        {
            return ShouldMap(type) && ShouldMap(type.BaseType);
        }
    }
}