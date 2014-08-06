using System.Collections.Generic;
using System.Linq;
using MrCMS.ACL;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class ACLModel
    {
        private ACLModel()
        {
        }

        public List<ACLRoleModel> Roles { get; private set; }
        public List<ACLRuleModel> Features { get; private set; }
        public List<ACLRuleModel> Webpages { get; private set; }
        public List<ACLRuleModel> Widgets { get; private set; }

        public static ACLModel Create(IEnumerable<UserRole> userRoles, List<ACLRule> systemRules)
        {
            List<ACLRoleModel> roles =
                userRoles.Select(role => new ACLRoleModel { Name = role.Name, Role = role }).ToList();
            List<ACLRuleModel> featureRules = systemRules.SelectMany(rule => rule.GetRules()
                .Select(aclGroup => ACLRuleModel.Create(roles, rule, aclGroup))).ToList();

            var typeACL = new TypeACLRule();

            List<ACLRuleModel> webpageRules = typeACL.WebpageRules
                .Select(
                    aclGroup =>
                        ACLRuleModel.Create(roles, typeACL, aclGroup)).ToList();
            List<ACLRuleModel> widgetRules = typeACL.WidgetRules
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
    }
}