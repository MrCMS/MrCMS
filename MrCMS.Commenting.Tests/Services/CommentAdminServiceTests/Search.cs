using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MrCMS.Commenting.Tests.Support;
using MrCMS.Paging;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Website;
using NHibernate.Event;
using Xunit;
using Xunit.Extensions;

namespace MrCMS.Commenting.Tests.Services.CommentAdminServiceTests
{
    public class Search : InMemoryDatabaseTest
    {
        [Theory]
        [InlineData(1, 0, 10)]
        [InlineData(2, 10, 10)]
        [InlineData(3, 20, 10)]
        [InlineData(4, 30, 9)]
        public void ReturnsAPagedListOfComments(int page, int skip, int take)
        {
            var comments = new List<Comment>();
            39.Times(() => Session.SaveAndAddEntityTo(new CommentBuilder().Build(), comments));
            CommentAdminService adminService = new CommentAdminServiceBuilder().WithSite(CurrentSite).WithSession(Session).Build();
            var commentSearchQuery = new CommentSearchQuery { Page = page };

            IPagedList<Comment> pagedList = adminService.Search(commentSearchQuery);

            pagedList.Should().BeEquivalentTo(comments.Skip(skip).Take(take));
        }

        [Theory]
        [InlineData(ApprovalStatus.Approved, 0, 2)]
        [InlineData(ApprovalStatus.Rejected, 2, 2)]
        [InlineData(ApprovalStatus.Pending, 4, 2)]
        [InlineData(ApprovalStatus.Any, 0, 6)]
        public void FiltersByApprovalStatusIfSet(ApprovalStatus approved, int skip, int take)
        {
            var comments = new List<Comment>();
            2.Times(() => Session.SaveAndAddEntityTo(new CommentBuilder().IsApproved().Build(), comments));
            2.Times(() => Session.SaveAndAddEntityTo(new CommentBuilder().IsNotApproved().Build(), comments));
            2.Times(() => Session.SaveAndAddEntityTo(new CommentBuilder().IsPending().Build(), comments));
            CommentAdminService adminService = new CommentAdminServiceBuilder().WithSite(CurrentSite).WithSession(Session).Build();
            var commentSearchQuery = new CommentSearchQuery {ApprovalStatus = approved};

            IPagedList<Comment> pagedList = adminService.Search(commentSearchQuery);

            pagedList.Should().BeEquivalentTo(comments.Skip(skip).Take(take));
        }

        [Fact]
        public void FiltersWithMatchAnyWhereLikeQueryOnEmailIfSet()
        {
            20.Times(
                i =>
                Session.SaveAndAddEntityTo(new CommentBuilder().WithEmail(i + "@example.com").Build()));

            CommentAdminService adminService = new CommentAdminServiceBuilder().WithSite(CurrentSite).WithSession(Session).Build();
            var commentSearchQuery = new CommentSearchQuery { Email = "1@exam" };

            IPagedList<Comment> pagedList = adminService.Search(commentSearchQuery);

            pagedList.Should().HaveCount(2);
            pagedList[0].Email.Should().Be("1@example.com");
            pagedList[1].Email.Should().Be("11@example.com");
        }

        [Fact]
        public void FiltersWithMatchAnyWhereLikeQueryOnMessageIfSet()
        {
            20.Times(
                i =>
                Session.SaveAndAddEntityTo(
                    new CommentBuilder().WithMessage(string.Format("This is comment #{0} in the list", i)).Build()));
            CommentAdminService adminService = new CommentAdminServiceBuilder().WithSite(CurrentSite).WithSession(Session).Build();
            var commentSearchQuery = new CommentSearchQuery { Message = "1 in the " };

            IPagedList<Comment> pagedList = adminService.Search(commentSearchQuery);

            pagedList.Should().HaveCount(2);
            pagedList[0].Message.Should().Be("This is comment #1 in the list");
            pagedList[1].Message.Should().Be("This is comment #11 in the list");
        }

        [Fact]
        public void FiltersByDateFromIfItIsSet()
        {
            // We are disabling MrCMS.DbConfiguration.Configuration.SaveOrUpdateListener here, 
            // to allow us to control the CreatedOn of the comments
            Session.GetSessionImplementation().Listeners.PreInsertEventListeners = new IPreInsertEventListener[0];
            var today = CurrentRequestData.Now.Date;
            var comments = new List<Comment>();
            10.Times(i => Session.SaveAndAddEntityTo(new CommentBuilder().WithSite(CurrentSite).WithCreatedOn(today.AddDays(i)).Build(), comments));

            CommentAdminService adminService = new CommentAdminServiceBuilder().WithSite(CurrentSite).WithSession(Session).Build();
            var commentSearchQuery = new CommentSearchQuery { DateFrom = today.AddDays(5) };

            IPagedList<Comment> pagedList = adminService.Search(commentSearchQuery);

            pagedList.Should().BeEquivalentTo(comments.Skip(5));
        }

        [Fact]
        public void FiltersByDateToIfItIsSet()
        {
            // We are disabling MrCMS.DbConfiguration.Configuration.SaveOrUpdateListener here, 
            // to allow us to control the CreatedOn of the comments
            Session.GetSessionImplementation().Listeners.PreInsertEventListeners = new IPreInsertEventListener[0];
            var today = CurrentRequestData.Now.Date;
            var comments = new List<Comment>();
            10.Times(i => Session.SaveAndAddEntityTo(new CommentBuilder().WithSite(CurrentSite).WithCreatedOn(today.AddDays(i)).Build(), comments));

            CommentAdminService adminService = new CommentAdminServiceBuilder().WithSite(CurrentSite).WithSession(Session).Build();
            var commentSearchQuery = new CommentSearchQuery {DateTo = today.AddDays(5)};

            IPagedList<Comment> pagedList = adminService.Search(commentSearchQuery);

            pagedList.Should().BeEquivalentTo(comments.Take(5));
        }
    }
}