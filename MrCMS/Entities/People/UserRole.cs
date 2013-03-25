using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.People
{
    public class UserRole : SystemEntity
    {
        public UserRole()
        {
            FrontEndWebpages = new List<Webpage>();
            AdminWebpages = new List<Webpage>();
        }
        public const string Administrator = "Administrator";

        [Required]
        [DisplayName("Role Name")]
        public virtual string Name { get; set; }

        public virtual IList<User> Users { get; set; }

        public virtual bool IsAdmin { get { return Name == Administrator; } }

        public virtual IList<Webpage> FrontEndWebpages { get; set; }
        public virtual IList<Webpage> AdminWebpages { get; set; }
    }
}
