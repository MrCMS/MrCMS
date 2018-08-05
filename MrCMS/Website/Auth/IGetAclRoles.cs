using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.ACL;

namespace MrCMS.Website.Auth
{
    public interface IGetAclRoles
    {
        Task<List<ACLRole>> GetRoles(IList<string> roles, IList<string> keys);
    }
}