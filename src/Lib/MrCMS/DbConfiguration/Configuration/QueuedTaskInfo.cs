using System;

namespace MrCMS.DbConfiguration.Configuration
{
    public struct QueuedTaskInfo
    {
        public Type Type { get; set; }
        public string Data { get; set; }
        public int SiteId { get; set; }
    }
}