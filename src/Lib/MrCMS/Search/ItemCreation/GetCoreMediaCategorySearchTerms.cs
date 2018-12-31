using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;

namespace MrCMS.Search.ItemCreation
{
    public class GetCoreMediaCategorySearchTerms : IGetMediaCategorySearchTerms
    {
        public IEnumerable<string> GetPrimary(MediaCategory mediaCategory)
        {
            yield return mediaCategory.Name;
        }

        public Dictionary<MediaCategory, HashSet<string>> GetPrimary(HashSet<MediaCategory> mediaCategories)
        {
            return mediaCategories.ToDictionary(category => category, category => GetPrimary(category).ToHashSet());
        }

        public IEnumerable<string> GetSecondary(MediaCategory mediaCategory)
        {
            yield break;
        }

        public Dictionary<MediaCategory, HashSet<string>> GetSecondary(HashSet<MediaCategory> mediaCategories)
        {
            return mediaCategories.ToDictionary(category => category, category => GetSecondary(category).ToHashSet());
        }
    }
}