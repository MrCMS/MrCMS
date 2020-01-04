using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;

namespace MrCMS.Entities.Resources
{
    public class StringResource : SystemEntity
    {
        [Required]
        public virtual string Key { get; set; }
        [Required]
        public virtual string Value { get; set; }

        public virtual Site Site { get; set; }
        public int? SiteId { get; set; }

        public virtual string UICulture { get; set; }

        public virtual string DisplayUICulture
        {
            get { return IsDefaultUICulture ? "Default" : CultureInfo.GetCultureInfo(UICulture).DisplayName; }
        }
        public virtual string DisplaySite
        {
            get { return IsGlobal ? "System Default" : Site.DisplayName; }
        }
        public virtual bool IsDefault
        {
            get { return IsDefaultUICulture && IsGlobal; }
        }

        public virtual bool IsDefaultUICulture
        {
            get { return string.IsNullOrWhiteSpace(UICulture); }
        }
        public virtual bool IsGlobal
        {
            get { return Site == null; }
        }
    }
}