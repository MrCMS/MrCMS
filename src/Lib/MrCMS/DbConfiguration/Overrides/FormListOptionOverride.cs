using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.DbConfiguration.Overrides
{
    public class FormListOptionOverride : IAutoMappingOverride<FormListOption>
    {
        public void Override(AutoMapping<FormListOption> mapping)
        {
            mapping.References(option => option.FormProperty).Column("FormPropertyId");
        }
    }
}