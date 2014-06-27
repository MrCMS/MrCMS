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