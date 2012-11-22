using System.Collections.Generic;
using System.ComponentModel;

namespace MrCMS.Entities.Documents.Web
{
    public interface IDocumentContainer<out T> where T : IContainerItem
    {
        string LiveUrlSegment { get; }
        string BodyContent { get; }

        [DisplayName("Page Size")]
        int PageSize { get; }

        [DisplayName("Allow Paging")]
        bool AllowPaging { get; }

        IList<Tag> Tags { get; }

        IEnumerable<T> ChildItems { get; }
        string DocumentType { get; }
    }
}