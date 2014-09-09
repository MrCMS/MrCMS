using System.Collections.Generic;

namespace MrCMS.Settings
{
    public class NHibernateConfigurationData : SystemSettingsBase
    {
        public NHibernateConfigurationData()
        {
            Entities = new HashSet<string>();
        }
        public byte[] Data { get; set; }
        public HashSet<string> Entities { get; set; }
    }
}