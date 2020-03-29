using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class ACLModel
    {
        public List<ACLRoleModel> Roles { get; set; }
        public List<ACLRuleModel> Features { get; set; }
        public List<ACLRuleModel> Webpages { get; set; }
        public List<ACLRuleModel> Widgets { get; set; }
    }
}