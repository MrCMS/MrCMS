using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.DbConfiguration.Overrides
{
    public class FormPropertyWithOptionsOverride : IAutoMappingOverride<FormPropertyWithOptions>
    {
        public void Override(AutoMapping<FormPropertyWithOptions> mapping)
        {
            mapping.HasMany(options => options.Options).KeyColumn("FormPropertyId");
        }
    }
}