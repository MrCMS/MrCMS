using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides
{
    public class FormOverride : IAutoMappingOverride<Form>
    {
        public void Override(AutoMapping<Form> mapping)
        {
            mapping.HasMany(form => form.FormPostings).Cascade.Delete().Cache.ReadWrite();
        }
    }
}