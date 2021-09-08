using System;
using Microsoft.Extensions.Logging;
//using Elmah;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;

//using MrCMS.Website;


namespace MrCMS.Logging
{
    //[AdminUISiteAgnostic]
    public class Log : SystemEntity
    {
        public virtual LogEntryType Type { get; set; }

        //public virtual Error Error { get; set; }
        public virtual string ExceptionData { get; set; }
        public virtual string RequestData { get; set; }
        public virtual string Message { get; set; }
        public virtual string Detail { get; set; }

        public virtual string DetailFormatted => (Detail ?? string.Empty).Replace(Environment.NewLine, "<br />");

        public virtual LogLevel LogLevel { get; set; }

        public virtual Site Site { get; set; }
        public virtual string SiteName => Site?.DisplayName;
    }
}