using System;

namespace MrCMS.Settings
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DropDownSelectionAttribute : Attribute
    {
        public DropDownSelectionAttribute(string viewDataName)
        {
            ViewDataName = viewDataName;
        }

        public string ViewDataName { get; set; }
    }
}