using System;
using System.Linq;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class DocumentTagsAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly DocumentTagsAdminService _documentTagsAdminService;

        public DocumentTagsAdminServiceTests()
        {
            _documentTagsAdminService = new DocumentTagsAdminService(Session);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_IfDocumentIsNullThrowArgumentNullException()
        {
            _documentTagsAdminService.Invoking(service => service.SetTags(null, null)).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_IfTagsIsNullForANewDocumentTheTagListShouldBeEmpty()
        {
            var textPage = new StubWebpage();

            _documentTagsAdminService.SetTags(null, textPage);

            textPage.Tags.Should().HaveCount(0);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_IfTagsHasOneStringTheTagListShouldHave1Tag()
        {
            var textPage = new StubWebpage();

            _documentTagsAdminService.SetTags("test tag", textPage);

            textPage.Tags.Should().HaveCount(1);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_IfTagsHasTwoCommaSeparatedTagsTheTagListShouldHave2Tags()
        {
            var textPage = new StubWebpage();

            _documentTagsAdminService.SetTags("test 1, test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldTrimTagNames()
        {
            var textPage = new StubWebpage();

            _documentTagsAdminService.SetTags("test 1, test 2", textPage);

            textPage.Tags.ElementAt(1).Name.Should().Be("test 2");
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldAddTagsToDocument()
        {
            var textPage = new StubWebpage();

            _documentTagsAdminService.SetTags("test 1, test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldNotRecreateTags()
        {
            var textPage = new StubWebpage();
            var tag1 = new Tag {Name = "test 1"};
            var tag2 = new Tag {Name = "test 2"};
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);

            Session.Transact(session =>
            {
                session.SaveOrUpdate(textPage);
                session.SaveOrUpdate(tag1);
                session.SaveOrUpdate(tag2);
            });

            _documentTagsAdminService.SetTags(textPage.TagList, textPage);

            Session.QueryOver<Tag>().RowCount().Should().Be(2);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldNotReaddSetTags()
        {
            var textPage = new StubWebpage();
            var tag1 = new Tag {Name = "test 1"};
            var tag2 = new Tag {Name = "test 2"};
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);

            Session.Transact(session =>
            {
                session.SaveOrUpdate(textPage);
                session.SaveOrUpdate(tag1);
                session.SaveOrUpdate(tag2);
            });

            _documentTagsAdminService.SetTags(textPage.TagList, textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldRemoveTagsNotIncluded()
        {
            var textPage = new StubWebpage();
            var tag1 = new Tag {Name = "test 1"};
            var tag2 = new Tag {Name = "test 2"};
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);

            Session.Transact(session =>
            {
                session.SaveOrUpdate(textPage);
                session.SaveOrUpdate(tag1);
                session.SaveOrUpdate(tag2);
            });

            _documentTagsAdminService.SetTags("test 1", textPage);

            textPage.Tags.Should().HaveCount(1);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldAssignDocumentToTag()
        {
            var textPage = new StubWebpage();
            Session.Transact(session => session.SaveOrUpdate(textPage));

            _documentTagsAdminService.SetTags("test 1", textPage);

            var tags = textPage.Tags;
            tags.Should().HaveCount(1);
            tags.First().Documents.Should().HaveCount(1);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldRemoveTheDocumentFromTags()
        {
            var textPage = new StubWebpage();
            var tag1 = new Tag {Name = "test 1"};
            var tag2 = new Tag {Name = "test 2"};
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);
            tag1.Documents.Add(textPage);
            tag2.Documents.Add(textPage);

            Session.Transact(session =>
            {
                session.SaveOrUpdate(textPage);
                session.SaveOrUpdate(tag1);
                session.SaveOrUpdate(tag2);
            });

            _documentTagsAdminService.SetTags("test 1", textPage);

            tag1.Documents.Should().HaveCount(1);
            tag2.Documents.Should().HaveCount(0);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldNotCreateTagsWithEmptyNames()
        {
            var textPage = new StubWebpage();

            _documentTagsAdminService.SetTags("test 1,,test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldNotCreateTagsWithEmptyNamesForTrailingComma()
        {
            var textPage = new StubWebpage();

            _documentTagsAdminService.SetTags("test 1, test 2, ", textPage);

            textPage.Tags.Should().HaveCount(2);
        }
    }
}