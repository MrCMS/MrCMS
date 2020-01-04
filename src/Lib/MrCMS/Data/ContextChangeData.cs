using System.Collections.Generic;

namespace MrCMS.Data
{
    public class ContextChangeData
    {
        public ICollection<EntityData> Added { get; set; }
        public ICollection<ChangeInfo> Updated { get; set; }
        public ICollection<EntityData> Deleted { get; set; }
    }
}