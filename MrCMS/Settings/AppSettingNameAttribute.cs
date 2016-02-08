using System;

namespace MrCMS.Settings
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AppSettingNameAttribute : Attribute
    {
        public AppSettingNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}