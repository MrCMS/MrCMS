using System;

namespace MrCMS.Settings
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConnectionStringAttribute : Attribute
    {
        public string Name { get; private set; }

        public ConnectionStringAttribute(string name)
        {
            Name = name;
        }
    }
}