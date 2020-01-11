using MrCMS.Entities.People;

namespace MrCMS.Entities.Documents.Web
{
    public class FrontEndAllowedRole : SiteEntity, IRole
    {
        public virtual Webpage Webpage { get; set; }
        public int WebpageId { get; set; }
        public virtual UserRole UserRole { get; set; }
        public int UserRoleId { get; set; }
        public virtual bool? IsRecursive { get; set; }
    }
}