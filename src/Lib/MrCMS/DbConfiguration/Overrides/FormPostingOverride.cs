using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides
{
    public class FormPostingOverride : IAutoMappingOverride<FormPosting>
    {
        public void Override(AutoMapping<FormPosting> mapping)
        {
            mapping.HasMany(posting => posting.FormValues).Cascade.All();
        }
    }
}