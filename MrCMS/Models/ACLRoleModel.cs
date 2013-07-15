using MrCMS.Entities.People;

namespace MrCMS.Models
{
    public class ACLRoleModel
    {
        internal ACLRoleModel()
        {

        }
        public string Name { get; set; }

        internal UserRole Role { get; set; }
    }
}