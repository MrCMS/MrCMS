using System.Collections.Generic;

namespace MrCMS.Web.Admin.Models
{
    public class ACLRuleOperationModel
    {
        //public static ACLRuleOperationModel Create(List<ACLRoleModel> roles, ACLRule rule, ACLOperation operation, Type type)
        //{
        //    return new ACLRuleOperationModel
        //               {
        //                   DisplayName = operation.Name,
        //                   Key = operation.Key,
        //                   Roles = roles.Select(role => ACLAvailableModel.Create(role, rule, operation.Name, type)).ToList()
        //               };
        //}

        //private ACLRuleOperationModel()
        //{

        //}
        public string DisplayName { get; set; }
        public List<ACLAvailableModel> Roles { get; set; }
        public string Key { get; set; }
    }
}