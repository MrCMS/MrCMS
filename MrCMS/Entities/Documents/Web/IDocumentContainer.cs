using System.Collections.Generic;
using System.ComponentModel;

namespace MrCMS.Entities.Documents.Web
{
    public interface IDocumentContainer
    {
        string LiveUrlSegment { get; }
        string BodyContent { get; }

        [DisplayName("Page Size")]
        int PageSize { get; }

        [DisplayName("Allow Paging")]
        bool AllowPaging { get; }

        IList<Tag> Tags { get; }

        string DocumentType { get; }
    }
    public interface IDocumentContainer<out T> : IDocumentContainer where T : IContainerItem
    {
        IEnumerable<T> ChildItems { get; }
    }
}