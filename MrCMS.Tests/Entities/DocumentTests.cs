using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Xunit;
using MrCMS.Helpers;
using System.Linq;

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
            var version1 = new DocumentVersion {CreatedOn = CurrentRequestData.Now};
            var version2 = new DocumentVersion {CreatedOn = CurrentRequestData.Now.AddDays(1)};
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
            var document1 = new BasicMappedNoChildrenInNavWebpage();
            var document2 = new BasicMappedNoChildrenInNavWebpage {PublishOn = DateTime.Today.AddDays(-1)};
            var document3 = new BasicMappedWebpage();

            Session.Transact(session =>
                {
                    session.Save(document1);
                    session.Save(document2);
                    session.Save(document3);
                });

            Webpage webpageAlias = null;
            DocumentTypeCount countAlias = null;
            var list =
                Session.QueryOver(()=>webpageAlias)
                       .SelectList(
                           builder =>
                           builder.SelectGroup(() => webpageAlias.DocumentType)
                                  .WithAlias(() => countAlias.DocumentType)
                                  .SelectCount(() => webpageAlias.Id)
                                  .WithAlias(() => countAlias.Total)
                                  .SelectSubQuery(QueryOver.Of<Webpage>().Where(webpage => webpage.DocumentType == webpageAlias.DocumentType && (webpage.PublishOn == null || webpage.PublishOn > CurrentRequestData.Now)).ToRowCountQuery())
                                  .WithAlias(() => countAlias.Unpublished))
                       .TransformUsing(Transformers.AliasToBean<DocumentTypeCount>())
                       .List<DocumentTypeCount>();

            list.Should().HaveCount(2);
            list.Sum(count => count.Unpublished).Should().Be(2);

        }
    }
    public class DocumentTypeCount
    {
        public string DocumentType { get; set; }
        public int Total { get; set; }
        public int Unpublished { get; set; }
    }
}