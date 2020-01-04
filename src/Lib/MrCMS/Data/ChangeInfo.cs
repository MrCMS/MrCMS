using System.Collections.Generic;

namespace MrCMS.Data
{
    public class ChangeInfo : EntityData
    {
        public List<PropertyData> PropertiesUpdated { get; set; }
    }
}