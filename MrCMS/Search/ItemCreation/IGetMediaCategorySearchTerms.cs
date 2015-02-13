using System.Collections.Generic;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.Search.ItemCreation
{
    public interface IGetMediaCategorySearchTerms
    {
        IEnumerable<string> Get(MediaCategory mediaCategory);
        Dictionary<MediaCategory, HashSet<string>> Get(HashSet<MediaCategory> mediaCategories);
    }
}