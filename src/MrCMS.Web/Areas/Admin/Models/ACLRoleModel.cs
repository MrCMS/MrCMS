using MrCMS.Entities.People;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class ACLRoleModel
    {
        internal ACLRoleModel()
        {

        }
        public string Name { get; set; }

        internal Role Role { get; set; }
    }
}