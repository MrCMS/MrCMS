using System;

namespace MrCMS.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AuditableAttribute : Attribute
    {
        public AuditOperations Operations { get; set; } = AuditOperations.AllSimple;
    }
}