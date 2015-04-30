using System;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class MessageTemplateInfo
    {
        public Type Type { get; set; }
        public string TypeName { get { return Type.FullName; } }
        public bool CanPreview { get; set; }
        public bool IsOverride { get; set; }
        public bool IsEnabled { get; set; }
        public bool LegacyTemplateExists { get; set; }
    }
}