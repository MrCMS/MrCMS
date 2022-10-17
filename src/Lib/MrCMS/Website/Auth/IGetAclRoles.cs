using System.Collections.Generic;
using MrCMS.Entities.ACL;

namespace MrCMS.Website.Auth
{
    public interface IGetAclRoles
    {
        IReadOnlyList<ACLRole> GetRoles(IEnumerable<string> roles, IEnumerable<string> keys);
    }
}