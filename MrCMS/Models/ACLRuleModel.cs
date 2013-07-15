using System.Collections.Generic;
using System.Linq;
using MrCMS.ACL;

namespace MrCMS.Models
{
    public class ACLRuleModel
    {
        public static ACLRuleModel Create(List<ACLRoleModel> roles, ACLRule rule, string key, List<ACLOperation> operations, Dictionary<string, string> customData)
        {
            return new ACLRuleModel
                       {
                           Name = key,
                           Operations = operations.Select(operation => ACLRuleOperationModel.Create(roles, rule, operation,customData)).ToList()
                       };
        }

        internal ACLRuleModel()
        {

        }
        public string Name { get; set; }
        public List<ACLRuleOperationModel> Operations { get; set; }
    }
}