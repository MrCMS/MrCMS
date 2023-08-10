using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MrCMS.Entities.ACL;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Website.Auth;

public class AclRoleRepository : IAclRoleRepository
{
    private readonly ISession _session;
    private readonly IMemoryCache _cache;

    // Semaphore used for async lock
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    public AclRoleRepository(ISession session, IMemoryCache cache)
    {
        _session = session;
        _cache = cache;
    }

    public async Task<ISet<AclRoleModel>> GetAllAsync()
    {
        // First check, not thread safe
        if (_cache == null) return null;

        var cachedData = _cache.Get<HashSet<AclRoleModel>>("aclRoles");
        if (cachedData != null)
        {
            return cachedData;
        }

        await Semaphore.WaitAsync();

        try
        {
            // Second check, thread safe
            cachedData = _cache.Get<HashSet<AclRoleModel>>("aclRoles");
            if (cachedData != null)
            {
                return cachedData;
            }

            var data = await _session.Query<ACLRole>().Select(role => new AclRoleModel
            {
                RoleId = role.UserRole.Id,
                Name = role.Name
            }).ToListAsync();

            var set = data.ToHashSet();

            _cache.Set("aclRoles", set, DateTimeOffset.Now.AddHours(12));

            return set;
        }
        finally
        {
            Semaphore.Release();
        }
    }


    public async Task<ISet<ACLRole>> GetAllEntitiesAsync()
    {
        var roles = await _session.Query<ACLRole>().ToListAsync();
        return roles.ToHashSet();
    }


    public async Task ModifyAsync(ICollection<ACLRole> aclRolesToAdd, ICollection<ACLRole> aclRolesToDelete)
    {
        await Semaphore.WaitAsync();
        try
        {
            await _session.TransactAsync(async x =>
            {
                foreach (var role in aclRolesToDelete)
                    await x.DeleteAsync(role);
                foreach (var role in aclRolesToAdd)
                    await x.SaveAsync(role);
            });
            _cache.Remove("aclRoles");
        }
        finally
        {
            Semaphore.Release();
        }
    }
}