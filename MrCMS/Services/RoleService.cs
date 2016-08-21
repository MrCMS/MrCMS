using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using NHibernate;
using NHibernate.Criterion;
using System.Linq;
using MrCMS.Data;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRepository<UserRole> _roleRepository;

        public RoleService(IRepository<UserRole> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public void Add(UserRole role)
        {
            _roleRepository.Add(role);
        }
        public void Update(UserRole role)
        {
            _roleRepository.Update(role);
        }

        public IEnumerable<UserRole> GetAllRoles()
        {
            return _roleRepository.Query().ToList();
        }

        public UserRole GetRoleByName(string name)
        {
            return _roleRepository.Query()
                .SingleOrDefault(role => role.Name == name);
        }

        public void DeleteRole(UserRole role)
        {
            if (!role.IsAdmin)
                _roleRepository.Delete(role);
        }

        public bool IsOnlyAdmin(User user)
        {
            var adminRole = GetRoleByName(UserRole.Administrator);

            var users = adminRole.Users.Where(user1 => user1.IsActive).Distinct().ToList();
            return users.Count() == 1 && users.First() == user;
        }

        public IEnumerable<AutoCompleteResult> Search(string term)
        {
            var userRoles =
                _roleRepository.Query().ToList();
            if (!string.IsNullOrWhiteSpace(term))
                userRoles = userRoles.FindAll(x => x.Name.StartsWith(term, StringComparison.OrdinalIgnoreCase));
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
            return _roleRepository.Get(id);
        }
    }
}