using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Interaction;

namespace MrCMS.DbConfiguration.Overrides
{
    public class ContactUsOverride : IAutoMappingOverride<ContactUs>
    {
        public void Override(AutoMapping<ContactUs> mapping)
        {
            mapping.Map(us => us.Subject).Length(1000);
            mapping.Map(us => us.Message).Length(4001); //4001 > == nvarcharmax
        }
    }
}