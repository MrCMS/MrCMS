using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.ACL;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Admin.Models;
using NHibernate.Linq;
using ISession = NHibernate.ISession;

namespace MrCMS.Web.Admin.Services
{
    public class AclAdminService : IAclAdminService
    {
        private readonly IGetAclOptions _getAclInfos;
        private readonly IRoleManager _roleManager;
        private readonly ISession _session;

        public AclAdminService(
            IGetAclOptions getAclInfos,
            IRoleManager roleManager,
            ISession session
        )
        {
            _getAclInfos = getAclInfos;
            _roleManager = roleManager;
            _session = session;
        }

        public async Task<List<AclInfo>> GetOptions()
        {
            var infos = _getAclInfos.GetInfos();

            var roles = await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync();
            var aclRoles = await _session.Query<ACLRole>().ToListAsync();

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

            var aclRoles = await _session.Query<ACLRole>().ToListAsync();

            var toAdd = records.Where(
                x => !aclRoles.Any(aclRole => aclRole.UserRole?.Id == x.RoleId && aclRole.Name == x.Key));
            var toRemove =
                aclRoles.Where(x => !records.Any(record => record.RoleId == x.UserRole?.Id && record.Key == x.Name));

            await _session.TransactAsync(async session =>
            {
                foreach (var record in toAdd)
                {
                    await session.SaveAsync(new ACLRole {Name = record.Key, UserRole = GetRole(record.RoleId)});
                }

                foreach (var aclRole in toRemove)
                {
                    await session.DeleteAsync(aclRole);
                }
            });
            return true;
        }

        private UserRole GetRole(int id)
        {
            return _session.Get<UserRole>(id);
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