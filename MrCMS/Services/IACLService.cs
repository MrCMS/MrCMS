using System.Collections.Generic;
using MrCMS.ACL;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IACLService
    {
        ACLModel GetACLModel();
        List<ACLRule> GetAllSystemRules();
        void UpdateACL(List<ACLUpdateRecord> model);
    }
}