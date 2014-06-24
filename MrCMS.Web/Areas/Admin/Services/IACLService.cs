using System.Collections.Generic;
using MrCMS.ACL;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IACLService
    {
        ACLModel GetACLModel();
        List<ACLRule> GetAllSystemRules();
        void UpdateACL(List<ACLUpdateRecord> model);
    }
}