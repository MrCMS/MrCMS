using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Services;

using Xunit;
using MrCMS.Helpers;
using MrCMS.TestSupport;

namespace MrCMS.Tests.Services
{
    public class RoleServiceTests : MrCMSTest
    {
        private RoleService _roleService;
        private IGlobalRepository<UserRole> _repository;

        public RoleServiceTests()
        {
            _repository = A.Fake<IGlobalRepository<UserRole>>();
            _roleService = new RoleService(_repository);
        }

        [Fact]
        public void RoleService_GetAllRoles_ReturnsAllRolesSavedToSession()
        {
            var roles = Enumerable.Range(1, 10).Select(i => new UserRole { Name = "Role " + i });
            A.CallTo(() => _repository.Query()).ReturnsAsAsyncQueryable(roles.ToArray());

            _roleService.GetAllRoles().Should().HaveCount(10);
        }

        [Fact]
        public void RoleService_GetAllRoles_ReturnsTheRolesThatWereSavedInOrder()
        {
            var roles = Enumerable.Range(1, 10).Select(i => new UserRole { Name = "Role " + i });
            A.CallTo(() => _repository.Query()).ReturnsAsAsyncQueryable(roles.ToArray());

            _roleService.GetAllRoles().Should().OnlyContain(role => roles.Contains(role));
        }

        [Fact]
        public async Task RoleService_AddRole_PersistsRoleToTheSession()
        {
            var userRole = new UserRole();

            await _roleService.AddRole(userRole);

            A.CallTo(() => _repository.Add(userRole, default)).MustHaveHappened();
        }

        [Fact]
        public async Task RoleService_UpdateRole_PersistsRoleToTheSession()
        {
            var userRole = new UserRole();

            await _roleService.UpdateRole(userRole);

            A.CallTo(() => _repository.Update(userRole, default)).MustHaveHappened();
        }

        [Fact]
        public void RoleService_GetRoleByName_ShouldReturnTheRoleWithTHeMatchingName()
        {
            var userRoles = Enumerable.Range(1, 10).Select(i => new UserRole { Name = "Role " + i }).ToList();
            A.CallTo(() => _repository.Query()).ReturnsAsAsyncQueryable(userRoles.ToArray());

            var roleByName = _roleService.GetRoleByName("Role 3");

            roleByName.Should().BeSameAs(userRoles[2]);
        }

        [Fact]
        public async Task RoleService_DeleteRole_ShouldDeleteAStandardRole()
        {
            var userRole = new UserRole { Name = "Standard Role" };

            await _roleService.DeleteRole(userRole);

            A.CallTo(() => _repository.Delete(userRole, default)).MustHaveHappened();
        }

        [Fact]
        public async Task RoleService_DeleteRole_ShouldNotDeleteAdminRole()
        {
            var userRole = new UserRole { Name = "Administrator" };

            await _roleService.DeleteRole(userRole);

            A.CallTo(() => _repository.Delete(userRole, default)).MustNotHaveHappened();
        }

        [Fact]
        public void RoleService_IsOnlyAdmin_ShouldBeTrueWhenThereIsOnly1AdminUser()
        {
            var admin = new User { IsActive = true };
            var userToRoles = new List<UserToRole>();
            var userRole = new UserRole { Name = "Administrator", UserToRoles = userToRoles };
            userToRoles.Add(new UserToRole { User = admin, Role = userRole });
            A.CallTo(() => _repository.Query()).ReturnsAsAsyncQueryable(userRole);

            var isOnlyAdmin = _roleService.IsOnlyAdmin(admin);

            isOnlyAdmin.Should().BeTrue();
        }

        [Fact]
        public void RoleService_IsOnlyAdmin_ShouldBeFalseWhenThereIsMoreThan1AdminUser()
        {
            var admin1 = new User { IsActive = true };
            var admin2 = new User { IsActive = true };
            var userToRoles = new List<UserToRole>();
            var userRole = new UserRole { Name = "Administrator", UserToRoles = userToRoles };
            userToRoles.Add(new UserToRole { User = admin1, Role = userRole });
            userToRoles.Add(new UserToRole { User = admin2, Role = userRole });
            A.CallTo(() => _repository.Query()).ReturnsAsAsyncQueryable(userRole);

            var isOnlyAdmin = _roleService.IsOnlyAdmin(admin1);

            isOnlyAdmin.Should().BeFalse();
        }

        [Fact]
        public void RoleService_Search_ShouldReturnAllRolesIfNoTermIsSet()
        {
            var roles = Enumerable.Range(1, 9).Select(i => new UserRole { Name = "Role " + i });

            A.CallTo(() => _repository.Readonly()).ReturnsAsAsyncQueryable(roles.ToArray());

            _roleService.Search(null).Should().HaveCount(9);
        }

        [Fact]
        public void RoleService_Search_ShouldFilterByTermPassedIn()
        {
            var roles = Enumerable.Range(1, 9).Select(i => new UserRole { Name = "Role " + i });

            A.CallTo(() => _repository.Readonly()).ReturnsAsAsyncQueryable(roles.ToArray());

            _roleService.Search("Role 3").Should().HaveCount(1);
        }

        [Fact]
        public void RoleService_Search_ShouldFilterByTermPassedCaseInsensitive()
        {
            var roles = Enumerable.Range(1, 9).Select(i => new UserRole { Name = "Role " + i });

            A.CallTo(() => _repository.Readonly()).ReturnsAsAsyncQueryable(roles.ToArray());

            _roleService.Search("roLE 3").Should().HaveCount(1);
        }
    }
}