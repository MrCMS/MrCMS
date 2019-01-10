using System.Collections.Generic;

namespace MrCMS.Models
{
    public class ACLUpdateRecord
    {
        public string Role { get; set; }
        public string Key { get; set; }
        public bool Allowed { get; set; }
    }
}