using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.ACL;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.People
{
    public class Role : SystemEntity
    {
        public Role()
        {
            FrontEndAllowedRoles = new List<FrontEndAllowedRole>();
            ACLRoles = new List<ACLRole>();
            UserRoles = new List<UserToRole>();
        }
        public const string Administrator = "Administrator";

        [Required]
        [DisplayName("Role Name")]
        public virtual string Name { get; set; }

        public virtual IList<UserToRole> UserRoles { get; set; }

        public virtual bool IsAdmin { get { return Name == Administrator; } }

        public virtual IList<FrontEndAllowedRole> FrontEndAllowedRoles { get; set; }
        public virtual IList<ACLRole> ACLRoles { get; set; }
    }
}
