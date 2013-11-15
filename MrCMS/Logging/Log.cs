using System;
using Elmah;
using MrCMS.Entities;
using MrCMS.Website;

namespace MrCMS.Logging
{
    [AdminUISiteAgnostic]
    public class Log : SiteEntity
    {
        public virtual LogEntryType Type { get; set; }
        public virtual Guid Guid { get; set; }
        public virtual Error Error { get; set; }
        public virtual string Message { get; set; }
        public virtual string Detail { get; set; }

        public virtual string DetailFormatted
        {
            get { return (Detail ?? string.Empty).Replace(Environment.NewLine, "<br />"); }
        }
    }
}