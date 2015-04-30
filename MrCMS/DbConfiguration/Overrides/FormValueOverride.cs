using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides
{
    public class FormValueOverride : IAutoMappingOverride<FormValue>
    {
        public void Override(AutoMapping<FormValue> mapping)
        {
            mapping.Map(posting => posting.Value).MakeVarCharMax();
            mapping.Map(posting => posting.Key).MakeVarCharMax();
        }
    }
}