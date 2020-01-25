using MrCMS.Entities.People;

namespace MrCMS.Entities.ACL
{
    public class ACLRole : SiteEntity
    {
        public virtual Role UserRole { get; set; }
        public int UserRoleId { get; set; }

        public virtual string Name { get; set; }
    }
}