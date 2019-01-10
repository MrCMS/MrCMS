using System;

namespace MrCMS.Website
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AclAttribute : Attribute
    {
        public AclAttribute(Type type, string operation)
        {
            Type = type;
            Operation = operation;
        }

        public Type Type { get; }
        public string Operation { get; }
        public string Name => Type?.Name;

        public bool ReturnEmptyResult { get; set; }
    }
}