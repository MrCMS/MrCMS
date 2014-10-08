using System.Collections.Generic;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.Search.ItemCreation
{
    public class GetCoreMediaCategorySearchTerms : IGetMediaCategorySearchTerms
    {
        public IEnumerable<string> Get(MediaCategory mediaCategory)
        {
            yield return mediaCategory.Name;
        }
    }
}