using System.Collections.Generic;
using MrCMS.Entities.ACL;

namespace MrCMS.Website.Auth
{
    public interface IGetAclRoles
    {
        List<ACLRole> GetRoles(IList<string> roles, IList<string> keys);
    }
}