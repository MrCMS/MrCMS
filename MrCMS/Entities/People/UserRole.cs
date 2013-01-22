using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.People
{
    public class UserRole : SystemEntity
    {
        public const string Administrator = "Administrator";

        [Required]
        [DisplayName("Role Name")]
        public virtual string Name { get; set; }

        public virtual IList<User> Users { get; set; }

        public virtual bool IsAdmin { get { return Name == Administrator; } }
    }
}
