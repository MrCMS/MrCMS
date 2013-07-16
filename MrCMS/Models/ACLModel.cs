using System.Collections.Generic;
using System.Linq;
using MrCMS.ACL;
using MrCMS.Services;

namespace MrCMS.Models
{
    public class ACLModel
    {
        public static ACLModel Create(IRoleService roleService, IACLService aclService)
        {
            var userRoles = roleService.GetAllRoles().ToList();
            var roles = userRoles.Select(role => new ACLRoleModel { Name = role.Name, Role = role }).ToList();
            var featureRules = aclService.GetAllSystemRules()
                          .SelectMany(rule => rule.GetRules()
                          .Select(aclGroup => ACLRuleModel.Create(roles, rule, aclGroup)))
                          .ToList();

            var typeACL = new TypeACLRule();

            var webpageRules = typeACL.WebpageRules
                                         .Select(
                                             aclGroup =>
                                             ACLRuleModel.Create(roles, typeACL, aclGroup)).ToList();
            var widgetRules = typeACL.WidgetRules
                                       .Select(
                                           aclGroup =>
                                           ACLRuleModel.Create(roles, typeACL, aclGroup)).ToList();
            return new ACLModel
                       {
                           Roles = roles,
                           Features = featureRules,
                           Webpages = webpageRules,
                           Widgets = widgetRules,
                       };
        }

        private ACLModel()
        {
        }
        public List<ACLRoleModel> Roles { get; private set; }
        public List<ACLRuleModel> Features { get; private set; }
        public List<ACLRuleModel> Webpages { get; private set; }
        public List<ACLRuleModel> Widgets { get; private set; }
    }
}