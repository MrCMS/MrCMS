using System;
using System.ComponentModel.DataAnnotations;
using MrCMS.Services;

namespace MrCMS.Messages
{
    public abstract class MessageTemplateBase : IStoredInAppData
    {
        [Required]
        [Display(Name = "From Address")]
        public string FromAddress { get; set; }

        [Required]
        [Display(Name = "From Name")]
        public string FromName { get; set; }

        [Required]
        [Display(Name = "To Address")]
        public string ToAddress { get; set; }

        [Display(Name = "To Name")]
        public string ToName { get; set; }

        public string Cc { get; set; }
        public string Bcc { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        [Display(Name = "Is HTML?")]
        public bool IsHtml { get; set; }

        
        public virtual Type ModelType
        {
            get { return null; }
        }

        public int? SiteId { get; set; }
    }

    public abstract class MessageTemplateBase<T> : MessageTemplateBase
    {
        public override sealed Type ModelType
        {
            get { return typeof (T); }
        }
    }
}