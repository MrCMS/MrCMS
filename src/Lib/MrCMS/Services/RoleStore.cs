using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class RoleStore : IQueryableRoleStore<UserRole>
    {
        private readonly ISession _session;

        public RoleStore(ISession session)
        {
            _session = session;
        }
        public void Dispose()
        {
        }

        public async Task<IdentityResult> CreateAsync(UserRole role, CancellationToken cancellationToken)
        {
            await _session.TransactAsync((session, token) => session.SaveAsync(role, token), cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(UserRole role, CancellationToken cancellationToken)
        {
            await _session.TransactAsync((session, token) => session.UpdateAsync(role, token), cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(UserRole role, CancellationToken cancellationToken)
        {
            await _session.TransactAsync((session, token) => session.DeleteAsync(role, token), cancellationToken);
            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(UserRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role?.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(UserRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role?.Name);
        }

        public Task SetRoleNameAsync(UserRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return UpdateAsync(role, cancellationToken);
        }

        public Task<string> GetNormalizedRoleNameAsync(UserRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role?.Name);
        }

        public Task SetNormalizedRoleNameAsync(UserRole role, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<UserRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return int.TryParse(roleId, out int id) ? _session.GetAsync<UserRole>(id, cancellationToken) : Task.FromResult<UserRole>(null);
        }

        public Task<UserRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return _session.Query<UserRole>().WithOptions(x => x.SetCacheable(true))
                .FirstOrDefaultAsync(x => x.Name == normalizedRoleName, cancellationToken);
        }

        public IQueryable<UserRole> Roles => _session.Query<UserRole>().WithOptions(x => x.SetCacheable(true));
    }
}