using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Paging;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using Xunit;

namespace MrCMS.Commenting.Tests.Admin.Controllers.CommentControllerTests
{
    public class Index
    {
        [Fact]
        public void ReturnsAViewResult()
        {
            new CommentControllerBuilder().Build().Index(null).Should().NotBeNull();
        }

        [Fact]
        public void ReturnsThePassedQueryAsTheViewModel()
        {
            var controller = new CommentControllerBuilder().Build();
            var commentSearchQuery = new CommentSearchQuery();

            var result = controller.Index(commentSearchQuery);

            result.Model.Should().Be(commentSearchQuery);
        }

        [Fact]
        public void ViewDataResultsShouldBeTheResultOfAdminServiceSearchWithQuery()
        {
            var commentSearchQuery = new CommentSearchQuery();
            var commentAdminService = A.Fake<ICommentAdminService>();
            var results = PagedList<Comment>.Empty;
            A.CallTo(() => commentAdminService.Search(commentSearchQuery)).Returns(results);
            var controller = new CommentControllerBuilder().WithAdminService(commentAdminService).Build();

            var result = controller.Index(commentSearchQuery);

            result.ViewData["results"].Should().Be(results);
        }

        [Fact]
        public void ViewDataApprovalOptionsShouldBeTheResultOfAdminServiceQuery()
        {
            var commentAdminService = A.Fake<ICommentAdminService>();
            List<SelectListItem> options = new List<SelectListItem>();
            A.CallTo(() => commentAdminService.GetApprovalOptions()).Returns(options);
            var controller = new CommentControllerBuilder().WithAdminService(commentAdminService).Build();

            var result = controller.Index(A<CommentSearchQuery>._);

            result.ViewData["approval-options"].Should().Be(options);
        }
    }
}