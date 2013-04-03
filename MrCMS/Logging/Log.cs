using System;
using Elmah;
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
}