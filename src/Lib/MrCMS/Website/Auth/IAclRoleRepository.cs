using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.ACL;

namespace MrCMS.Website.Auth;

public interface IAclRoleRepository
{
    Task<ISet<AclRoleModel>> GetAllAsync();
    Task<ISet<ACLRole>> GetAllEntitiesAsync();
    Task ModifyAsync(ICollection<ACLRole> aclRolesToAdd, ICollection<ACLRole> aclRolesToDelete);
}