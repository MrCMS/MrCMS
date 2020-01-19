using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class RoleStore : IQueryableRoleStore<Role>//, IRoleClaimStore<UserRole>
    {
        private readonly IGlobalRepository<Role> _repository;

        public RoleStore(IGlobalRepository<Role> repository)
        {
            _repository = repository;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            await _repository.Add(role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            await _repository.Update(role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            await _repository.Delete(role);
            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role?.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role?.Name);
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return UpdateAsync(role, cancellationToken);
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role?.Name);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return int.TryParse(roleId, out int id) ? _repository.Load(id) : Task.FromResult<Role>(null);
        }

        public Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return _repository.Query()
                .FirstOrDefaultAsync(x => x.Name == normalizedRoleName, cancellationToken);
        }

        public IQueryable<Role> Roles => _repository.Query<Role>();
    }
}