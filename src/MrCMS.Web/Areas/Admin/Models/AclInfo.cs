using System.Collections.Generic;
using MrCMS.Website.Auth;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class AclInfo
    {
        public AclType Type { get; set; }
        public string Rule { get; set; }
        public string Operation { get; set; }
        public string Key => AclKeyGenerator.GetKey(Type, Rule, Operation);
        public List<AclRoleInfo> Roles { get; set; }
    }
}