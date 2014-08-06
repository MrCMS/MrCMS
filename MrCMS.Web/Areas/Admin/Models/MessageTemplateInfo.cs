using System;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class MessageTemplateInfo
    {
        public Type Type { get; set; }
        public int? Id { get; set; }
        public bool CanPreview { get; set; }
    }
}