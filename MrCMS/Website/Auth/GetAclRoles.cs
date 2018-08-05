using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.ACL;
using MrCMS.Services;
using NHibernate.Linq;

namespace MrCMS.Website.Auth
{
    public class GetAclRoles : IGetAclRoles
    {
        private readonly IRepository<ACLRole> _repository;
        private readonly IRoleManager _roleManager;

        public GetAclRoles(IRepository<ACLRole> repository, IRoleManager roleManager)
        {
            _repository = repository;
            _roleManager = roleManager;
        }

        public async Task<List<ACLRole>> GetRoles(IList<string> roles, IList<string> keys)
        {
            var roleIds = await _roleManager.Roles.Where(role => roles.Contains(role.Name))
                .Select(x => x.Id)
                .ToListAsync();

            return await _repository.Query().Where(role => roleIds.Contains(role.UserRole.Id) && keys.Contains(role.Name))
                .ToListAsync();
        }
    }
}