using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Tests.Stubs;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Helpers
{
    public class DocumentExtensionsTests
    {
        [Fact]
        public void DocumentExtensions_SetParent_SetsTheParentDocument()
        {
            var child = new StubDocument();
            var parent = new StubDocument();

            child.SetParent(parent);

            child.Parent.Should().Be(parent);
        }

        [Fact]
        public void DocumentExtensions_SetParent_AddsTheChildToTheChildrenCollection()
        {
            var child = new StubDocument();
            var parent = new StubDocument();

            child.SetParent(parent);

            parent.Children.Should().Contain(child);
        }

        [Fact]
        public void DocumentExtensions_SetParent_RemovesTheChildFromTheOldParentsChildren()
        {
            var child = new StubDocument();
            var parent = new StubDocument();
            var oldParent = new StubDocument();
            oldParent.SetChildren(new List<Document> { child });
            child.Parent = oldParent;

            child.SetParent(parent);

            oldParent.Children.Should().NotContain(child);
        }

        [Fact]
        public void DocumentExtensions_GetVersion_ReturnsNullIfThereIsNoMatchingVersionInCollection()
        {
            var doc = new StubDocument();

            doc.GetVersion(1).Should().Be(null);
        }
    }
}