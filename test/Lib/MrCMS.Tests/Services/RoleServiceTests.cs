using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Services;
using Xunit;
using MrCMS.Helpers;
using MrCMS.TestSupport;

namespace MrCMS.Tests.Services
{
    public class RoleServiceTests : InMemoryDatabaseTest
    {
        private RoleService _roleService;

        public RoleServiceTests()
        {
            _roleService = new RoleService(Session);
        }

        [Fact]
        public async Task RoleService_GetAllRoles_ReturnsAllRolesSavedToSession()
        {
            foreach (var i in Enumerable.Range(1, 10))
            {
                var userRole = new UserRole {Name = "Role " + i};
                await Session.TransactAsync(session => session.SaveAsync(userRole));
            }

            var allRoles = await _roleService.GetAllRoles();

            allRoles.Should().HaveCount(10);
        }

        [Fact]
        public async Task RoleService_GetAllRoles_ReturnsTheRolesThatWereSavedInOrder()
        {
            var userRoles = new List<UserRole>();
            foreach (var i in Enumerable.Range(1, 10))
            {
                var userRole = new UserRole {Name = "Role " + i};
                userRoles.Add(userRole);
                await Session.TransactAsync(session => session.SaveAsync(userRole));
            }

            var allRoles = await _roleService.GetAllRoles();

            allRoles.Should().OnlyContain(role => userRoles.Contains(role));
        }

        [Fact]
        public async Task RoleService_SaveRole_PersistsRoleToTheSession()
        {
            await _roleService.SaveRole(new UserRole());

            (await Session.QueryOver<UserRole>().ListAsync()).Should().HaveCount(1);
        }

        [Fact]
        public async Task RoleService_GetRoleByName_ShouldReturnTheRoleWithTHeMatchingName()
        {
            var userRoles = new List<UserRole>();
            foreach (var i in Enumerable.Range(1, 10))
            {
                var userRole = new UserRole {Name = "Role " + i};
                userRoles.Add(userRole);
                await Session.TransactAsync(session => session.SaveAsync(userRole));
            }

            var roleByName = await _roleService.GetRoleByName("Role 3");

            roleByName.Should().BeSameAs(userRoles[2]);
        }

        [Fact]
        public async Task RoleService_DeleteRole_ShouldDeleteAStandardRole()
        {
            var userRole = new UserRole {Name = "Standard Role"};
            await Session.TransactAsync(session => session.SaveAsync(userRole));

            await _roleService.DeleteRole(userRole);

            (await Session.QueryOver<UserRole>().ListAsync()).Should().HaveCount(0);
        }

        [Fact]
        public async Task RoleService_DeleteRole_ShouldNotDeleteAdminRole()
        {
            var userRole = new UserRole {Name = "Administrator"};
            await Session.TransactAsync(session => session.SaveAsync(userRole));

            await _roleService.DeleteRole(userRole);

            (await Session.QueryOver<UserRole>().ListAsync()).Should().HaveCount(1);
        }

        [Fact]
        public async Task RoleService_IsOnlyAdmin_ShouldBeTrueWhenThereIsOnly1AdminUser()
        {
            var admin = new User {IsActive = true};
            var userRole = new UserRole {Name = "Administrator", Users = new HashSet<User> {admin}};
            await Session.TransactAsync(session => session.SaveAsync(userRole));

            var isOnlyAdmin = await _roleService.IsOnlyAdmin(admin);

            isOnlyAdmin.Should().BeTrue();
        }

        [Fact]
        public async Task RoleService_IsOnlyAdmin_ShouldBeFalseWhenThereIsMoreThan1AdminUser()
        {
            var admin1 = new User {IsActive = true};
            var admin2 = new User {IsActive = true};
            var userRole = new UserRole {Name = "Administrator", Users = new HashSet<User> {admin1, admin2}};
            await Session.TransactAsync(session => session.SaveAsync(userRole));

            var isOnlyAdmin = await _roleService.IsOnlyAdmin(admin1);

            isOnlyAdmin.Should().BeFalse();
        }

        [Fact]
        public async Task RoleService_Search_ShouldReturnAllRolesIfNoTermIsSet()
        {
            var userRoles = new List<UserRole>();
            foreach (var i in Enumerable.Range(1, 9))
            {
                var userRole = new UserRole {Name = "Role " + i};
                userRoles.Add(userRole);
                await Session.TransactAsync(session => session.SaveAsync(userRole));
            }

            var search = await _roleService.Search(null);
            search.Should().HaveCount(9);
        }

        [Fact]
        public async Task RoleService_Search_ShouldFilterByTermPassedIn()
        {
            var userRoles = new List<UserRole>();
            foreach (var i in Enumerable.Range(1, 9))
            {
                var userRole = new UserRole {Name = "Role " + i};
                userRoles.Add(userRole);
                await Session.TransactAsync(session => session.SaveAsync(userRole));
            }

            var search = await _roleService.Search("Role 3");
            
            search.Should().HaveCount(1);
        }

        [Fact]
        public async Task RoleService_Search_ShouldFilterByTermPassedCaseInsensitive()
        {
            var userRoles = new List<UserRole>();
            foreach (var i in Enumerable.Range(1, 9))
            {
                var userRole = new UserRole {Name = "Role " + i};
                userRoles.Add(userRole);
                await Session.TransactAsync(session => session.SaveAsync(userRole));
            }

            var search = await _roleService.Search("roLE 3");
            
            search.Should().HaveCount(1);
        }
    }
}