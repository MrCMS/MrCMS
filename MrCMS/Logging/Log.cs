using System;
using Elmah;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities;

namespace MrCMS.Logging
{
    public class Log : SiteEntity
    {
        public virtual LogEntryType Type { get; set; }
        public virtual Guid Guid { get; set; }
        public virtual Error Error { get; set; }
        public virtual string Message { get; set; }
        public virtual string Detail { get; set; }
    }

    public enum LogEntryType
    {
        Error,
        Audit
    }

    public class LogEntryOverride : IAutoMappingOverride<Log>
    {
        public void Override(AutoMapping<Log> mapping)
        {
            mapping.Map(entry => entry.Error).CustomType<BinaryData>().Length(9999);
            mapping.Map(entry => entry.Message).CustomType<VarcharMax>().Length(4001);
            mapping.Map(entry => entry.Detail).CustomType<VarcharMax>().Length(4001);
        }
    }
}