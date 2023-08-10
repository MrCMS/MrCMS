using System.Collections.Generic;

namespace MrCMS.Entities.Documents.Web;

public interface IContentBlock
{
    IReadOnlyList<BlockItem> Items { get; }
}
