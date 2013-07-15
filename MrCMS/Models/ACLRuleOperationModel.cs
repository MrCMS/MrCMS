using System.Collections.Generic;
using System.Linq;
using MrCMS.ACL;

namespace MrCMS.Models
{
    public class ACLRuleOperationModel
    {
        public static ACLRuleOperationModel Create(List<ACLRoleModel> roles, ACLRule rule, ACLOperation operation, Dictionary<string, string> customData)
        {
            return new ACLRuleOperationModel
                       {
                           DisplayName = operation.Name,
                           Key = operation.OperationKey,
                           Roles = roles.Select(role => ACLAvailableModel.Create(role, rule, operation.Name, customData)).ToList()
                       };
        }

        private ACLRuleOperationModel()
        {

        }
        public string DisplayName { get; set; }
        public List<ACLAvailableModel> Roles { get; set; }
        public string Key { get; set; }
    }
}