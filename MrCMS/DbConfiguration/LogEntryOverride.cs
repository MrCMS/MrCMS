using Elmah;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Logging;

namespace MrCMS.DbConfiguration
{
    public class LogEntryOverride : IAutoMappingOverride<Log>
    {
        public void Override(AutoMapping<Log> mapping)
        {
            mapping.Map(entry => entry.Error).CustomType<BinaryData<Error>>().Length(9999);
            mapping.Map(entry => entry.Message).CustomType<VarcharMax>().Length(4001);
            mapping.Map(entry => entry.Detail).CustomType<VarcharMax>().Length(4001);
        }
    }
}