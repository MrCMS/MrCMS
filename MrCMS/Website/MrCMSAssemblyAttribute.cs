using System;

namespace MrCMS.Website
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MrCMSAssemblyAttribute : Attribute
    {
        public MrCMSAssemblyAttribute(string version)
        {
            Version = version;
        }

        public string Version { get; set; }
    }
}