using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Models
{
    public class ACLAvailableModel
    {
        public static ACLAvailableModel Create(ACLRoleModel role, ACLRule rule, string operation, Dictionary<string, string> customData)
        {
            return new ACLAvailableModel
                       {
                           Role = role,
                           IsAllowed = rule.CanAccess(role.Role, operation, customData)
                       };
        }

        internal ACLAvailableModel()
        {

        }
        public ACLRoleModel Role { get; set; }
        public bool IsAllowed { get; set; }

        public string RoleName
        {
            get { return Role != null ? Role.Name : string.Empty; }
        }
    }
}