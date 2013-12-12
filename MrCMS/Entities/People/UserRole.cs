using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Iesi.Collections.Generic;
using MrCMS.Entities.ACL;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.People
{
    public class UserRole : SystemEntity
    {
        public UserRole()
        {
            FrontEndWebpages = new HashedSet<Webpage>();
            ACLRoles = new List<ACLRole>();
            Users = new HashedSet<User>();
        }
        public const string Administrator = "Administrator";

        [Required]
        [DisplayName("Role Name")]
        public virtual string Name { get; set; }

        public virtual Iesi.Collections.Generic.ISet<User> Users { get; set; }

        public virtual bool IsAdmin { get { return Name == Administrator; } }

        public virtual Iesi.Collections.Generic.ISet<Webpage> FrontEndWebpages { get; set; }
        public virtual IList<ACLRole> ACLRoles { get; set; }
    }
}
