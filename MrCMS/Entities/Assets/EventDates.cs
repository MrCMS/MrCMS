using System;

namespace MrCMS.Entities.Assets
{
    public class EventDates : SiteEntity
    {
        public virtual string Name { get; set; }
        public virtual DateTime DateFrom { get; set; }
        public virtual DateTime DateTo { get; set; }
    }
}
