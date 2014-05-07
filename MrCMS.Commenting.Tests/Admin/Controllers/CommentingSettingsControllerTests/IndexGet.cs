using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Web.Apps.Commenting.Settings;
using Xunit;
using FluentAssertions;

namespace MrCMS.Commenting.Tests.Admin.Controllers.CommentingSettingsControllerTests
{
    public class IndexGet
    {
        [Fact]
        public void ReturnsAViewResult()
        {
            new CommentingSettingsControllerBuilder().Build().Index().Should().NotBeNull();
        }

        [Fact]
        public void HasConfigProvidersCommentingSettingsAsModel()
        {
            var settings = new CommentingSettings();
            var adminService = A.Fake<ICommentingSettingsAdminService>();
            var controller = new CommentingSettingsControllerBuilder().WithAdminService(adminService).Build();
            A.CallTo(() => adminService.GetSettings()).Returns(settings);

            var viewResult = controller.Index();

            viewResult.Model.Should().Be(settings);
        }

        [Fact]
        public void ViewDataPageTypesShouldHavePageOptionsFromAdminService()
        {
            var pageTypes = new List<Type>();
            var commentingAdminService = A.Fake<ICommentingSettingsAdminService>();
            A.CallTo(() => commentingAdminService.GetAllPageTypes()).Returns(pageTypes);
            var controller = new CommentingSettingsControllerBuilder().WithAdminService(commentingAdminService).Build();

            var viewResult = controller.Index();

            viewResult.ViewData["page-types"].Should().Be(pageTypes);
        }

        [Fact]
        public void ViewDataCommentApprovalTypesShouldHaveOptionsFromAdminService()
        {
            List<SelectListItem> commentApprovalTypes = new List<SelectListItem>();
            var commentingAdminService = A.Fake<ICommentingSettingsAdminService>();
            A.CallTo(() => commentingAdminService.GetCommentApprovalTypes()).Returns(commentApprovalTypes);
            var controller = new CommentingSettingsControllerBuilder().WithAdminService(commentingAdminService).Build();
            
            var viewResult = controller.Index();

            viewResult.ViewData["comment-approval-types"].Should().Be(commentApprovalTypes);
        }
    }
}