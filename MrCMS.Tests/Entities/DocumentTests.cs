using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Tests.Stubs;
using NHibernate;
using NHibernate.Transform;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Entities
{
    public class DocumentTests : InMemoryDatabaseTest
    {
        [Fact]
        public void Document_CanDelete_IsTrueWhenNoChildren()
        {
            var doc = new StubDocument();

            doc.SetChildren(new List<Document>());

            doc.CanDelete.Should().BeTrue();
        }

        [Fact]
        public void Document_CanDelete_IsFalseWhenChildrenAreAdded()
        {
            var doc = new StubDocument();

            doc.SetChildren(new List<Document> {new StubDocument()});

            doc.CanDelete.Should().BeFalse();
        }

        [Fact]
        public void Document_PublishedChildren_DoesNotReturnNonWebpages()
        {
            var doc = new StubDocument();

            doc.SetChildren(new List<Document> {new StubDocument()});

            doc.PublishedChildren.Should().HaveCount(0);
        }

        [Fact]
        public void Document_PublishedChildren_PublishedWebpageIsReturned()
        {
            var doc = new StubDocument();

            var document = new BasicMappedWebpage {PublishOn = DateTime.UtcNow.AddDays(-1)};
            doc.SetChildren(new List<Document> {document});

            doc.PublishedChildren.Should().Contain(document);
        }

        [Fact]
        public void Document_PublishedChildren_UnpublishedWebpageIsNotReturned()
        {
            var doc = new StubDocument();

            var document = new BasicMappedWebpage {PublishOn = null};
            doc.SetChildren(new List<Document> {document});

            doc.PublishedChildren.Should().NotContain(document);
        }

        [Fact]
        public void Document_OnDeleting_RemovesDocumentFromParent()
        {
            var doc = new StubDocument();

            var child = new StubDocument();
            doc.SetChildren(new List<Document> {child});

            child.OnDeleting(A.Fake<ISession>());

            doc.Children.Should().NotContain(child);
        }

        [Fact]
        public void Document_GetVersions_ReturnsVersionsInDescendingCreatedOnOrder()
        {
            var document = new StubDocument();
            var version1 = new DocumentVersion {CreatedOn = DateTime.UtcNow};
            var version2 = new DocumentVersion {CreatedOn = DateTime.UtcNow.AddDays(1)};
            document.SetVersions(new List<DocumentVersion>
                {
                    version1,
                    version2
                });

            var versionsModel = document.GetVersions(1);

            versionsModel.Items.Should().ContainInOrder(new List<DocumentVersion> {version2, version1});
        }

        [Fact]
        public void Document_Grouping_CanGroupByDocumentType()
        {
            var document1 = new StubDocument();
            var document2 = new StubDocument();
            var document3 = new BasicMappedWebpage();

            Session.Transact(session =>
                {
                    session.Save(document1);
                    session.Save(document2);
                    session.Save(document3);
                });

            DocumentTypeCount alias = null;
            var list =
                Session.QueryOver<Document>()
                       .SelectList(
                           builder =>
                           builder.SelectGroup(document => document.DocumentType)
                                  .WithAlias(() => alias.DocumentType)
                                  .SelectCount(document => document.Id)
                                  .WithAlias(() => alias.Count))
                       .TransformUsing(Transformers.AliasToBean<DocumentTypeCount>())
                       .List<DocumentTypeCount>();

            list.Should().HaveCount(2);
            
        }
    }
    public class DocumentTypeCount
    {
        public string DocumentType { get; set; }
        public int Count { get; set; }
    }
}