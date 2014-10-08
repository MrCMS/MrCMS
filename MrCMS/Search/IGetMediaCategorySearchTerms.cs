using System.Collections.Generic;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.Search
{
    public interface IGetMediaCategorySearchTerms
    {
        IEnumerable<string> Get(MediaCategory mediaCategory);
    }
}