using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MrCMS.Entities.Multisite;

namespace MrCMS.Entities.Resources
{
    public class StringResource : SystemEntity
    {
        [Required]
        public virtual string Key { get; set; }
        [Required]
        public virtual string Value { get; set; }

        public virtual Site Site { get; set; }

        public virtual string UICulture { get; set; }

        public virtual string DisplayUICulture => IsDefaultUICulture ? "Default" : CultureInfo.GetCultureInfo(UICulture).DisplayName;

        public virtual string DisplaySite => IsGlobal ? "System Default" : Site.DisplayName;

        public virtual bool IsDefault => IsDefaultUICulture && IsGlobal;

        public virtual bool IsDefaultUICulture => string.IsNullOrWhiteSpace(UICulture);

        public virtual bool IsGlobal => Site == null;
    }
}