using MrCMS.Entities.People;

namespace MrCMS.Entities.Documents.Web
{
    public class AdminDisallowedRole : BaseEntity, IRole
    {
        public virtual Webpage Webpage { get; set; }
        public virtual UserRole UserRole { get; set; }
        public virtual bool? IsRecursive { get; set; }
    }
}