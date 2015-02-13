using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;

namespace MrCMS.Search.ItemCreation
{
    public class GetCoreMediaCategorySearchTerms : IGetMediaCategorySearchTerms
    {
        public IEnumerable<string> Get(MediaCategory mediaCategory)
        {
            yield return mediaCategory.Name;
        }

        public Dictionary<MediaCategory, HashSet<string>> Get(HashSet<MediaCategory> mediaCategories)
        {
            return mediaCategories.ToDictionary(category => category, category => Get(category).ToHashSet());
        }
    }
}