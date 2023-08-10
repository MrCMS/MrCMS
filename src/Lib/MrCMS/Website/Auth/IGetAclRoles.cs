using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.ACL;

namespace MrCMS.Website.Auth
{
    public interface IGetACLRoles
    {
        Task<bool> AnyRoles(ISet<int> roles, IEnumerable<string> keys);
    }

}