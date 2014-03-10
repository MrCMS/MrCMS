using System.Globalization;

namespace MrCMS.Entities.Resources
{
    public class StringResource : SiteEntity
    {
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public virtual string UICulture { get; set; }

        public virtual string DisplayUICulture
        {
            get { return IsDefault ? "Default" : CultureInfo.GetCultureInfo(UICulture).DisplayName; }
        }

        public virtual bool IsDefault
        {
            get { return string.IsNullOrWhiteSpace(UICulture); }
        }
    }
}