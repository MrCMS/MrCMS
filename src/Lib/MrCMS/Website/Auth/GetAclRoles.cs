using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MrCMS.Website.Auth
{
    public class GetAclRoles : IGetACLRoles
    {
        private readonly IAclRoleRepository _repository;

        public GetAclRoles(IAclRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AnyRoles(ISet<int> roles, IEnumerable<string> keys)
        {
            var allAsync = await _repository.GetAllAsync();
            return allAsync.Any(role => roles.Contains(role.RoleId) && keys.Contains(role.Name));
        }
    }


    public class AclRoleModel
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
    }
}