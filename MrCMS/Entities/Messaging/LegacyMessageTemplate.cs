using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Messaging
{
    public class LegacyMessageTemplate : SiteEntity
    {
        [Display(Name = "Message Template Type")]
        public virtual string MessageTemplateType { get; set; }

        [Required]
        [Display(Name = "From Address")]
        public virtual string FromAddress { get; set; }

        [Required]
        [Display(Name = "From Name")]
        public virtual string FromName { get; set; }

        [Required]
        [Display(Name = "To Address")]
        public virtual string ToAddress { get; set; }

        [Display(Name = "To Name")]
        public virtual string ToName { get; set; }

        public virtual string Cc { get; set; }
        public virtual string Bcc { get; set; }

        [Required]
        public virtual string Subject { get; set; }

        [Required]
        public virtual string Body { get; set; }

        [Required]
        [Display(Name = "Is HTML?")]
        public virtual bool IsHtml { get; set; }

        [Required]
        [Display(Name = "Is Imported?")]
        public virtual bool Imported { get; set; }
    }
}