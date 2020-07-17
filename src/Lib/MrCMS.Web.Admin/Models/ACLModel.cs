using System.Collections.Generic;
using System.Linq;
using MrCMS.ACL;
using MrCMS.Entities.People;

namespace MrCMS.Web.Admin.Models
{
    public class ACLModel
    {
        public List<ACLRoleModel> Roles { get; set; }
        public List<ACLRuleModel> Features { get; set; }
        public List<ACLRuleModel> Webpages { get; set; }
        public List<ACLRuleModel> Widgets { get; set; }
    }
}