using MrCMS.Entities.People;

namespace MrCMS.Entities.ACL
{
    public class ACLRole : SiteEntity
    {
        public virtual UserRole UserRole { get; set; }

        public virtual string Name { get; set; }
    }
}