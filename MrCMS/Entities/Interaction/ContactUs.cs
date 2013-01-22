using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Interaction
{
    public class ContactUs : SiteEntity
    {
        [Required]
        public virtual string Name { get; set; }
        [Required]
        public virtual string Email { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Phone { get; set; }
        [Required]
        public virtual string Message { get; set; }
    }
}