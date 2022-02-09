using System;

namespace MrCMS.Settings
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TextAreaAttribute : Attribute
    {
        public bool CKEnabled { get; set; }
    }
}