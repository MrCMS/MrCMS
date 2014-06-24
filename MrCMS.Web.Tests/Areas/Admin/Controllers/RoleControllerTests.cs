using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Services;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class RoleControllerTests
    {
        private IRoleAdminService roleService;

        [Fact]
        public void RoleController_Index_ReturnsViewResult()
        {
            RoleController roleController = GetRoleController();

            roleController.Index().Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void RoleController_Index_CallsRoleServiceGetAllRoles()
        {
            RoleController roleController = GetRoleController();

            roleController.Index();

            A.CallTo(() => roleService.GetAllRoles()).MustHaveHappened();
        }

        [Fact]
        public void RoleController_Index_ShouldReturnTheResultOfGetAllRoles()
        {
            RoleController roleController = GetRoleController();
            var userRoles = new List<UserRole>();
            A.CallTo(() => roleService.GetAllRoles()).Returns(userRoles);

            var result = roleController.Index().As<ViewResult>();

            result.Model.Should().Be(userRoles);
        }

        [Fact]
        public void RoleController_AddGet_ReturnsAPartialViewResult()
        {
            RoleController roleController = GetRoleController();

            roleController.Add().Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void RoleController_AddGet_ModelShouldBeAUserRole()
        {
            RoleController roleController = GetRoleController();

            roleController.Add().As<PartialViewResult>().Model.Should().BeOfType<UserRole>();
        }

        [Fact]
        public void RoleController_AddPost_ReturnsRedirectToRouteResult()
        {
            RoleController roleController = GetRoleController();
            var userRole = new UserRole();

            roleController.Add(userRole).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void RoleController_AddPost_ShouldCallAddRoleWithPassedRole()
        {
            RoleController roleController = GetRoleController();
            var userRole = new UserRole {Name = "test"};
            roleController.Add(userRole);

            A.CallTo(() => roleService.AddRole(userRole)).MustHaveHappened();
        }


        [Fact]
        public void RoleController_AddPost_ShouldRedirectToIndex()
        {
            RoleController roleController = GetRoleController();
            var userRole = new UserRole();

            var redirectToRouteResult = roleController.Add(userRole).As<RedirectToRouteResult>();

            redirectToRouteResult.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void RoleController_EditGet_IfUserRoleIsNullReturnsRedirectToIndex()
        {
            RoleController roleController = GetRoleController();

            ActionResult editGet = roleController.Edit_Get(null);

            editGet.Should().BeOfType<RedirectToRouteResult>();
            editGet.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void RoleController_EditGet_IfUserRoleIsSetShouldReturnViewResult()
        {
            RoleController roleController = GetRoleController();

            ActionResult editGet = roleController.Edit_Get(new UserRole());

            editGet.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void RoleController_EditGet_IfUserRoleIsSetViewResultModelShouldBeRolePassed()
        {
            RoleController roleController = GetRoleController();
            var userRole = new UserRole();

            ActionResult editGet = roleController.Edit_Get(userRole);

            editGet.As<ViewResult>().Model.Should().Be(userRole);
        }

        [Fact]
        public void RoleController_EditPost_ShouldReturnARedirectToRouteResult()
        {
            RoleController roleController = GetRoleController();
            var userRole = new UserRole();

            ActionResult edit = roleController.Edit(userRole);

            edit.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void RoleController_EditPost_ShouldCallSaveRoleOnTheRoleService()
        {
            RoleController roleController = GetRoleController();
            var userRole = new UserRole();

            roleController.Edit(userRole);

            A.CallTo(() => roleService.SaveRole(userRole)).MustHaveHappened();
        }

        [Fact]
        public void RoleController_EditPost_ShouldRedirectToIndex()
        {
            RoleController roleController = GetRoleController();
            var userRole = new UserRole();

            var redirectToRouteResult = roleController.Edit(userRole).As<RedirectToRouteResult>();

            redirectToRouteResult.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void RoleController_DeleteGet_NullRoleReturnsRedirectToIndex()
        {
            RoleController roleController = GetRoleController();

            ActionResult result = roleController.Delete_Get(null);

            result.Should().BeOfType<RedirectToRouteResult>();
            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void RoleController_DeleteGet_ReturnsViewResult()
        {
            RoleController roleController = GetRoleController();
            var userRole = new UserRole();

            ActionResult result = roleController.Delete_Get(userRole);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void RoleController_DeleteGet_ModelShouldBePassedRole()
        {
            RoleController roleController = GetRoleController();
            var userRole = new UserRole();

            ActionResult result = roleController.Delete_Get(userRole);

            result.As<ViewResult>().Model.Should().Be(userRole);
        }

        [Fact]
        public void RoleController_DeletePost_ReturnsRedirectToIndex()
        {
            RoleController roleController = GetRoleController();
            var userRole = new UserRole();

            roleController.Delete(userRole).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void RoleController_DeletePost_CallsRoleServiceDeleteRoleOnPassedRole()
        {
            RoleController roleController = GetRoleController();
            var userRole = new UserRole();

            roleController.Delete(userRole);

            A.CallTo(() => roleService.DeleteRole(userRole)).MustHaveHappened();
        }

        [Fact]
        public void RoleController_DeletePost_DoesNotCallDeleteRoleIfUserRoleIsNull()
        {
            RoleController roleController = GetRoleController();

            roleController.Delete(null);

            A.CallTo(() => roleService.DeleteRole(null)).MustNotHaveHappened();
        }

        private RoleController GetRoleController()
        {
            roleService = A.Fake<IRoleAdminService>();
            return new RoleController(roleService);
        }
    }
}