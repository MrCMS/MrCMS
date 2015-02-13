using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Iesi.Collections.Generic;
using Microsoft.AspNet.Identity;
using MrCMS.Entities.ACL;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.People
{
    public class UserRole : SystemEntity, IRole<int>
    {
        public UserRole()
        {
            FrontEndWebpages = new HashSet<Webpage>();
            ACLRoles = new List<ACLRole>();
            Users = new HashSet<User>();
        }
        public const string Administrator = "Administrator";

        [Required]
        [DisplayName("Role Name")]
        public virtual string Name { get; set; }

        public virtual ISet<User> Users { get; set; }

        public virtual bool IsAdmin { get { return Name == Administrator; } }

        public virtual ISet<Webpage> FrontEndWebpages { get; set; }
        public virtual IList<ACLRole> ACLRoles { get; set; }
    }
}
