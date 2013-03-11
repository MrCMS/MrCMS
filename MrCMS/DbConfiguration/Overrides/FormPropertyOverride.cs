using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides
{
    public class FormPropertyOverride : IAutoMappingOverride<FormProperty>
    {
        public void Override(AutoMapping<FormProperty> mapping)
        {
            mapping.DiscriminateSubClassesOnColumn("PropertyType");
        }
    }
}