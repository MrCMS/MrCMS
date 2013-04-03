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
        [Fact]
        public void RoleService_GetRole_CallsSessionGetRole()
        {
            var session = A.Fake<ISession>();
            var roleService = GetRoleService(session);

            roleService.GetRole(1);

            A.CallTo(() => session.Get<UserRole>(1)).MustHaveHappened();
        }

        [Fact]
        public void RoleService_GetRole_UsesTheResponseOfSessionGetRole()
        {
            var session = A.Fake<ISession>();
            var roleService = GetRoleService(session);
            var userRole = new UserRole();
            A.CallTo(() => session.Get<UserRole>(1)).Returns(userRole);

            roleService.GetRole(1).Should().Be(userRole);
        }

        [Fact]
        public void RoleService_GetAllRoles_ReturnsAllRolesSavedToSession()
        {
            var roleService = GetRoleService();

            Enumerable.Range(1,10).ForEach(i =>
                                               {
                                                   var userRole = new UserRole {Name = "Role " + i};
                                                   Session.Transact(session => session.Save(userRole));
                                               });

            roleService.GetAllRoles().Should().HaveCount(10);
        }

        [Fact]
        public void RoleService_GetAllRoles_ReturnsTheRolesThatWereSavedInOrder()
        {
            var roleService = GetRoleService();

            var userRoles = new List<UserRole>();
            Enumerable.Range(1,10).ForEach(i =>
                                               {
                                                   var userRole = new UserRole {Name = "Role " + i};
                                                   userRoles.Add(userRole);
                                                   Session.Transact(session => session.Save(userRole));
                                               });

            roleService.GetAllRoles().Should().OnlyContain(role => userRoles.Contains(role));
        }

        [Fact]
        public void RoleService_SaveRole_PersistsRoleToTheSession()
        {
            var roleService = GetRoleService();

            roleService.SaveRole(new UserRole());

            Session.QueryOver<UserRole>().List().Should().HaveCount(1);
        }

        [Fact]
        public void RoleService_GetRoleByName_ShouldReturnTheRoleWithTHeMatchingName()
        {
            var roleService = GetRoleService();
            var userRoles = Enumerable.Range(1, 10).Select(i => new UserRole {Name = "Role " + i}).ToList();
            Session.Transact(session => userRoles.ForEach(role => session.Save(role)));

            var roleByName = roleService.GetRoleByName("Role 3");

            roleByName.Should().BeSameAs(userRoles[2]);
        }

        [Fact]
        public void RoleService_DeleteRole_ShouldDeleteAStandardRole()
        {
            var roleService = GetRoleService();
            var userRole = new UserRole {Name = "Standard Role"};
            Session.Transact(session => session.Save(userRole));

            roleService.DeleteRole(userRole);

            Session.QueryOver<UserRole>().List().Should().HaveCount(0);
        }

        [Fact]
        public void RoleService_DeleteRole_ShouldNotDeleteAdminRole()
        {
            var roleService = GetRoleService();
            var userRole = new UserRole {Name = "Administrator"};
            Session.Transact(session => session.Save(userRole));

            roleService.DeleteRole(userRole);

            Session.QueryOver<UserRole>().List().Should().HaveCount(1);
        }

        [Fact]
        public void RoleService_IsOnlyAdmin_ShouldBeTrueWhenThereIsOnly1AdminUser()
        {
            var roleService = GetRoleService();
            var admin = new User { IsActive = true };
            var userRole = new UserRole { Name = "Administrator", Users = new List<User> { admin } };
            Session.Transact(session => session.Save(userRole));

            var isOnlyAdmin = roleService.IsOnlyAdmin(admin);

            isOnlyAdmin.Should().BeTrue();
        }

        [Fact]
        public void RoleService_IsOnlyAdmin_ShouldBeFalseWhenThereIsMoreThan1AdminUser()
        {
            var roleService = GetRoleService();
            var admin1 = new User { IsActive = true };
            var admin2 = new User { IsActive = true };
            var userRole = new UserRole { Name = "Administrator", Users = new List<User> { admin1,admin2 } };
            Session.Transact(session => session.Save(userRole));

            var isOnlyAdmin = roleService.IsOnlyAdmin(admin1);

            isOnlyAdmin.Should().BeFalse();
        }

        private RoleService GetRoleService(ISession session = null)
        {
            return new RoleService(session ?? Session);
        }
    }
}