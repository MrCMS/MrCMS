using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;

namespace MrCMS.Services
{
    public class RoleService : IRoleService
    {
        private readonly IGlobalRepository<UserRole> _repository;

        public RoleService(IGlobalRepository<UserRole> repository)
        {
            _repository = repository;
        }


        public Task AddRole(UserRole role)
        {
            return _repository.Add(role);
        }

        public Task UpdateRole(UserRole role)
        {
            return _repository.Update(role);
        }

        public IEnumerable<UserRole> GetAllRoles()
        {
            return _repository.Query().ToList();
        }

        public UserRole GetRoleByName(string name)
        {
            return _repository.Query().FirstOrDefault(x => x.Name == name);
        }

        public async Task DeleteRole(UserRole role)
        {
            if (!role.IsAdmin)
                await _repository.Delete(role);
        }

        public bool IsOnlyAdmin(User user)
        {
            var adminRole = GetRoleByName(UserRole.Administrator);

            var users = adminRole.UserToRoles.Select(y => y.User).Where(user1 => user1.IsActive).Distinct().ToList();
            return users.Count() == 1 && users.First() == user;
        }

        public IEnumerable<AutoCompleteResult> Search(string term)
        {
            var userRoles = _repository.Readonly().Where(x => EF.Functions.Like(x.Name, $"{term}%")).ToList();
            return
                userRoles.Select(
                    tag =>
                    new AutoCompleteResult
                    {
                        id = tag.Id,
                        label = tag.Name,
                        value = tag.Name
                    });
        }

        public UserRole GetRole(int id)
        {
            return _repository.LoadSync(id);
        }
    }
}