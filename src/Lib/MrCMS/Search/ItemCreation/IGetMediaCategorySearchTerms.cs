using System.Collections.Generic;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.Search.ItemCreation
{
    public interface IGetMediaCategorySearchTerms
    {
        IEnumerable<string> GetPrimary(MediaCategory mediaCategory);
        Dictionary<MediaCategory, HashSet<string>> GetPrimary(HashSet<MediaCategory> mediaCategories);
        IEnumerable<string> GetSecondary(MediaCategory mediaCategory);
        Dictionary<MediaCategory, HashSet<string>> GetSecondary(HashSet<MediaCategory> mediaCategories);
    }
}