using System;
using System.Linq;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Utils;
using MrCMS.Entities;
using MrCMS.Entities.Documents;

namespace MrCMS.DbConfiguration.Mapping
{
    public class MrCMSMappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.IsSubclassOf(typeof (SystemEntity)) &&
                   base.ShouldMap(type) && !HasDoNotMapAttribute(type);
        }

        private bool HasDoNotMapAttribute(Type type)
        {
            return type.GetCustomAttributes(typeof(DoNotMapAttribute), false).Any();
        }

        public override bool ShouldMap(FluentNHibernate.Member member)
        {
            return member.CanWrite && base.ShouldMap(member);
        }

        public override bool IsId(FluentNHibernate.Member member)
        {
            return member.Name == "Id" && base.IsId(member);
        }

        public override bool IsDiscriminated(System.Type type)
        {
            return type.IsSubclassOf(typeof(Document));
        }
    }
}