using System;

namespace MrCMS.Data
{
    [Flags]
    public enum AuditOperations
    {
        None = 0,
        Add = 1,
        Delete = 2,
        Update = 4,
        Properties = 8,
        Never = 256,
        All = Add | Delete | Update | Properties,
        AllSimple = Add | Delete | Update
    }
}