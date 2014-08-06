using System;
using MrCMS.ACL;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class ACLAvailableModel
    {
        public static ACLAvailableModel Create(ACLRoleModel role, ACLRule rule, string operation, Type type)
        {
            return new ACLAvailableModel
                       {
                           Role = role,
                           IsAllowed = rule.CanAccess(role.Role, operation, type == null ? null : type.FullName)
                       };
        }

        private ACLAvailableModel()
        {

        }
        public ACLRoleModel Role { get; private set; }
        public bool IsAllowed { get; private set; }

        public string RoleName
        {
            get { return Role != null ? Role.Name : string.Empty; }
        }
    }
}