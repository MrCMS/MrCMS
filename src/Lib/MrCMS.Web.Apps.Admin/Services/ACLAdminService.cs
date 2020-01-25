using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Entities.ACL;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class AclAdminService : IAclAdminService
    {
        private readonly IGetAclOptions _getAclInfos;
        private readonly IRoleManager _roleManager;
        private readonly IRepository<ACLRole> _aclRoleRepository;

        public AclAdminService(
            IGetAclOptions getAclInfos,
            IRoleManager roleManager,
            IRepository<ACLRole> aclRoleRepository
        )
        {
            _getAclInfos = getAclInfos;
            _roleManager = roleManager;
            _aclRoleRepository = aclRoleRepository;
        }

        public List<AclInfo> GetOptions()
        {
            var infos = _getAclInfos.GetInfos();

            var roles = _roleManager.Roles.OrderBy(x => x.Name).ToList();
            var aclRoles = _aclRoleRepository.Query().ToList();

            foreach (var info in infos)
                info.Roles = roles
                    .Select(role => new AclRoleInfo
                    {
                        Id = role.Id,
                        Name = role.Name,
                        Allowed = aclRoles.Any(x => x.UserRole?.Id == role.Id && x.Name == info.Key)
                    })
                    .ToList();

            return infos;
        }


        public async Task<bool> UpdateAcl(IFormCollection collection)
        {
            var records = GetUpdateRecords(collection["acl"]);

            var aclRoles = _aclRoleRepository.Query().ToList();

            var toAdd = records.Where(
                x => !aclRoles.Any(aclRole => aclRole.UserRole?.Id == x.RoleId && aclRole.Name == x.Key));
            var toRemove =
                aclRoles.Where(x => !records.Any(record => record.RoleId == x.UserRole?.Id && record.Key == x.Name));

            await _aclRoleRepository.Transact(async (repo, ct) =>
             {
                 await repo.AddRange(toAdd.Select(record => new ACLRole { Name = record.Key, UserRoleId = record.RoleId }).ToList(), ct);

                 await repo.DeleteRange(toRemove.ToList(), ct);
             });
            return true;
        }

        private static List<AclUpdateRecord> GetUpdateRecords(IList<string> values)
        {
            return values.Select(s =>
            {
                var parts = s.Split('-');

                if (parts.Length != 2)
                    return null;

                if (!int.TryParse(parts[0], out int id))
                    return null;

                return new AclUpdateRecord
                {
                    RoleId = id,
                    Key = parts[1]
                };
            }).Where(x => x != null).ToList();
        }
    }
}