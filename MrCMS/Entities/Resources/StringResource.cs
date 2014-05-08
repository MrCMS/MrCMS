using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MrCMS.Helpers;

namespace MrCMS.Entities.Resources
{
    public class StringResource : SiteEntity
    {
        [Required]
        public virtual string Key { get; set; }
        [Required]
        public virtual string Value { get; set; }
        public virtual string UICulture { get; set; }

        public virtual string DisplayUICulture
        {
            get { return IsDefault ? "Default" : CultureInfo.GetCultureInfo(UICulture).DisplayName; }
        }
        public virtual string DisplayKey
        {
            get
            {
                if (Key == null || Key.LastIndexOf(".", StringComparison.Ordinal) == -1)
                {
                    return Key;
                }
                var typeName = Key.Substring(0, Key.LastIndexOf(".", StringComparison.Ordinal));
                var type = TypeHelper.GetTypeByName(typeName);
                if (type != null)
                {
                    return type.Name.BreakUpString() + " - " +
                           Key.Substring(Key.LastIndexOf(".", StringComparison.Ordinal) + 1).BreakUpString();
                }
                return Key;
            }
        }

        public virtual bool IsDefault
        {
            get { return string.IsNullOrWhiteSpace(UICulture); }
        }
    }
}