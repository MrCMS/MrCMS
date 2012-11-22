using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities;

namespace MrCMS.AddOns.Newsletters
{
    [MrCMSMapClass]
    public class NewsletterSignup : BaseEntity
    {
        [Required]
        public virtual string Email { get; set; }

        [Required]
        public virtual string Name { get; set; }
    }
}