using MrCMS.Entities.People;

namespace MrCMS.Entities.ACL
{
    public class ACLRole : SiteEntity
    {
        public virtual Role Role { get; set; }
        public int RoleId { get; set; }

        public virtual string Name { get; set; }
    }
}