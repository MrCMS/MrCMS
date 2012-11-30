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

        private RoleService GetRoleService(ISession session = null)
        {
            return new RoleService(session ?? Session);
        }
    }
}