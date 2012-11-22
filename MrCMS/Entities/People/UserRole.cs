using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.People
{
    public class UserRole : BaseEntity
    {
        [Required]
        [DisplayName("Role Name")]
        public virtual string Name { get; set; }

        public virtual IList<User> Users { get; set; }
    }
}
