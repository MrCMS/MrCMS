using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Iesi.Collections.Generic;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Services;
using NHibernate;
using Xunit;
using MrCMS.Helpers;
using MrCMS.Tests.TestSupport;

namespace MrCMS.Tests.Services
{
    public class RoleServiceTests 
    {
        private RoleService _roleService;
        private IRepository<UserRole> _roleRepository = new InMemoryRepository<UserRole>();

        public RoleServiceTests()
        {
            _roleService = new RoleService(_roleRepository);
        }

        [Fact]
        public void RoleService_GetAllRoles_ReturnsAllRolesSavedToSession()
        {
            Enumerable.Range(1,10).ForEach(i =>
                                               {
                                                   var userRole = new UserRole {Name = "Role " + i};
                                                   _roleRepository.Add(userRole);
                                               });

            _roleService.GetAllRoles().Should().HaveCount(10);
        }

        [Fact]
        public void RoleService_GetAllRoles_ReturnsTheRolesThatWereSavedInOrder()
        {
            var userRoles = new List<UserRole>();
            Enumerable.Range(1,10).ForEach(i =>
                                               {
                                                   var userRole = new UserRole {Name = "Role " + i};
                                                   userRoles.Add(userRole);
                                                   _roleRepository.Add(userRole);
                                               });

            _roleService.GetAllRoles().Should().OnlyContain(role => userRoles.Contains(role));
        }

        [Fact]
        public void RoleService_SaveRole_PersistsRoleToTheSession()
        {
            _roleService.Add(new UserRole());

            _roleRepository.Query().ToList().Should().HaveCount(1);
        }

        [Fact]
        public void RoleService_GetRoleByName_ShouldReturnTheRoleWithTHeMatchingName()
        {
            var userRoles = Enumerable.Range(1, 10).Select(i => new UserRole {Name = "Role " + i}).ToList();
            userRoles.ForEach(role => _roleRepository.Add(role));

            var roleByName = _roleService.GetRoleByName("Role 3");

            roleByName.Should().BeSameAs(userRoles[2]);
        }

        [Fact]
        public void RoleService_DeleteRole_ShouldDeleteAStandardRole()
        {
            var userRole = new UserRole {Name = "Standard Role"};
            _roleRepository.Add(userRole);

            _roleService.DeleteRole(userRole);

            _roleRepository.Query().ToList().Should().HaveCount(0);
        }

        [Fact]
        public void RoleService_DeleteRole_ShouldNotDeleteAdminRole()
        {
            var userRole = new UserRole {Name = "Administrator"};
            _roleRepository.Add(userRole);

            _roleService.DeleteRole(userRole);

            _roleRepository.Query().ToList().Should().HaveCount(1);
        }

        [Fact]
        public void RoleService_IsOnlyAdmin_ShouldBeTrueWhenThereIsOnly1AdminUser()
        {
            var admin = new User { IsActive = true };
            var userRole = new UserRole { Name = "Administrator", Users = new HashSet<User> { admin } };
            _roleRepository.Add(userRole);

            var isOnlyAdmin = _roleService.IsOnlyAdmin(admin);

            isOnlyAdmin.Should().BeTrue();
        }

        [Fact]
        public void RoleService_IsOnlyAdmin_ShouldBeFalseWhenThereIsMoreThan1AdminUser()
        {
            var admin1 = new User { IsActive = true };
            var admin2 = new User { IsActive = true };
            var userRole = new UserRole { Name = "Administrator", Users = new HashSet<User> { admin1, admin2 } };
            _roleRepository.Add(userRole);

            var isOnlyAdmin = _roleService.IsOnlyAdmin(admin1);

            isOnlyAdmin.Should().BeFalse();
        }

        [Fact]
        public void RoleService_Search_ShouldReturnAllRolesIfNoTermIsSet()
        {
            Enumerable.Range(1, 9)
                      .Select(i => new UserRole {Name = "Role " + i})
                      .ForEach(role => _roleRepository.Add(role));

            _roleService.Search(null).Should().HaveCount(9);
        }

        [Fact]
        public void RoleService_Search_ShouldFilterByTermPassedIn()
        {
            Enumerable.Range(1, 9)
                      .Select(i => new UserRole {Name = "Role " + i})
                      .ForEach(role => _roleRepository.Add(role));

            _roleService.Search("Role 3").Should().HaveCount(1);
        }

        [Fact]
        public void RoleService_Search_ShouldFilterByTermPassedCaseInsensitive()
        {
            Enumerable.Range(1, 9)
                      .Select(i => new UserRole {Name = "Role " + i})
                      .ForEach(role => _roleRepository.Add(role));

            _roleService.Search("roLE 3").Should().HaveCount(1);
        }
    }
}