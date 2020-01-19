using MrCMS.Data;
using MrCMS.Entities.People;

namespace MrCMS.Entities.Documents.Web
{
    public class FrontEndAllowedRole : IJoinTable
    {
        public virtual Webpage Webpage { get; set; }
        public int WebpageId { get; set; }
        public virtual Role Role { get; set; }
        public int RoleId { get; set; }
    }
}