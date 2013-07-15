using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.ACL;
using MrCMS.Helpers;
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
                          .Select(pair => ACLRuleModel.Create(roles, rule, pair.Key, pair.Value, new Dictionary<string, string>())))
                          .ToList();
            var webpageACL = new WebpageACL();
            var webpageRules = webpageACL.GetRules()
                                         .Select(
                                             pair =>
                                             ACLRuleModel.Create(roles, webpageACL,
                                                                 TypeHelper.GetTypeByName(pair.Key).Name,
                                                                 pair.Value,
                                                                 new Dictionary<string, string> { { "page-type", pair.Key } }))
                                         .ToList();
            var widgetACL= new WidgetACL();
            var widgetRules = widgetACL.GetRules()
                                         .Select(
                                             pair =>
                                             ACLRuleModel.Create(roles, widgetACL,
                                                                 WidgetHelper.WidgetTypes.FirstOrDefault(type => type.FullName == pair.Key).Name,
                                                                 pair.Value,
                                                                 new Dictionary<string, string> { { "widget-type", pair.Key } }))
                                         .ToList();
            return new ACLModel
                       {
                           Roles = roles,
                           Features = featureRules,
                           Webpages = webpageRules,
                           Widgets = widgetRules
                       };
        }

        internal ACLModel()
        {
        }
        public List<ACLRoleModel> Roles { get; internal set; }

        public List<ACLRuleModel> Features { get; internal set; }
        public List<ACLRuleModel> Webpages { get; internal set; }
        public List<ACLRuleModel> Widgets { get; internal set; }
    }
}