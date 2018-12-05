//using Elmah;

using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Logging;

namespace MrCMS.DbConfiguration.Overrides
{
    public class LogOverride : IAutoMappingOverride<Log>
    {
        public void Override(AutoMapping<Log> mapping)
        {
            //mapping.Map(entry => entry.Error).CustomType<BinaryData<Error>>().Length(9999);
            mapping.Map(entry => entry.Message).MakeVarCharMax();
            mapping.Map(entry => entry.Detail).MakeVarCharMax();
        }
    }
}