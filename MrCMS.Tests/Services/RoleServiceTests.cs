using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Services;
using NHibernate;
using Xunit;
using MrCMS.Helpers;

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
        public void RoleService_GetAllRoles_ReturnsAllRolesSavedToSession()
        {
            Enumerable.Range(1,10).ForEach(i =>
                                               {
                                                   var userRole = new UserRole {Name = "Role " + i};
                                                   Session.Transact(session => session.Save(userRole));
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
                                                   Session.Transact(session => session.Save(userRole));
                                               });

            _roleService.GetAllRoles().Should().OnlyContain(role => userRoles.Contains(role));
        }

        [Fact]
        public void RoleService_SaveRole_PersistsRoleToTheSession()
        {
            _roleService.SaveRole(new UserRole());

            Session.QueryOver<UserRole>().List().Should().HaveCount(1);
        }

        [Fact]
        public void RoleService_GetRoleByName_ShouldReturnTheRoleWithTHeMatchingName()
        {
            var userRoles = Enumerable.Range(1, 10).Select(i => new UserRole {Name = "Role " + i}).ToList();
            Session.Transact(session => userRoles.ForEach(role => session.Save(role)));

            var roleByName = _roleService.GetRoleByName("Role 3");

            roleByName.Should().BeSameAs(userRoles[2]);
        }

        [Fact]
        public void RoleService_DeleteRole_ShouldDeleteAStandardRole()
        {
            var userRole = new UserRole {Name = "Standard Role"};
            Session.Transact(session => session.Save(userRole));

            _roleService.DeleteRole(userRole);

            Session.QueryOver<UserRole>().List().Should().HaveCount(0);
        }

        [Fact]
        public void RoleService_DeleteRole_ShouldNotDeleteAdminRole()
        {
            var userRole = new UserRole {Name = "Administrator"};
            Session.Transact(session => session.Save(userRole));

            _roleService.DeleteRole(userRole);

            Session.QueryOver<UserRole>().List().Should().HaveCount(1);
        }

        [Fact]
        public void RoleService_IsOnlyAdmin_ShouldBeTrueWhenThereIsOnly1AdminUser()
        {
            var admin = new User { IsActive = true };
            var userRole = new UserRole { Name = "Administrator", Users = new List<User> { admin } };
            Session.Transact(session => session.Save(userRole));

            var isOnlyAdmin = _roleService.IsOnlyAdmin(admin);

            isOnlyAdmin.Should().BeTrue();
        }

        [Fact]
        public void RoleService_IsOnlyAdmin_ShouldBeFalseWhenThereIsMoreThan1AdminUser()
        {
            var admin1 = new User { IsActive = true };
            var admin2 = new User { IsActive = true };
            var userRole = new UserRole { Name = "Administrator", Users = new List<User> { admin1,admin2 } };
            Session.Transact(session => session.Save(userRole));

            var isOnlyAdmin = _roleService.IsOnlyAdmin(admin1);

            isOnlyAdmin.Should().BeFalse();
        }

        [Fact]
        public void RoleService_Search_ShouldReturnAllRolesIfNoTermIsSet()
        {
            Enumerable.Range(1, 9)
                      .Select(i => new UserRole {Name = "Role " + i})
                      .ForEach(role => Session.Transact(session => session.Save(role)));

            _roleService.Search(null).Should().HaveCount(9);
        }

        [Fact]
        public void RoleService_Search_ShouldFilterByTermPassedIn()
        {
            Enumerable.Range(1, 9)
                      .Select(i => new UserRole {Name = "Role " + i})
                      .ForEach(role => Session.Transact(session => session.Save(role)));

            _roleService.Search("Role 3").Should().HaveCount(1);
        }

        [Fact]
        public void RoleService_Search_ShouldFilterByTermPassedCaseInsensitive()
        {
            Enumerable.Range(1, 9)
                      .Select(i => new UserRole {Name = "Role " + i})
                      .ForEach(role => Session.Transact(session => session.Save(role)));

            _roleService.Search("roLE 3").Should().HaveCount(1);
        }
    }
}