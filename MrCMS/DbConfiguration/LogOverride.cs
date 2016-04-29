using Elmah;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Logging;

namespace MrCMS.DbConfiguration
{
    public class LogOverride : IAutoMappingOverride<Log>
    {
        public void Override(AutoMapping<Log> mapping)
        {
            mapping.Map(entry => entry.Error).CustomType<BinaryData<Error>>().Length(16000000);
            mapping.Map(entry => entry.Message).MakeVarCharMax();
            mapping.Map(entry => entry.Detail).MakeVarCharMax();
        }
    }
}
