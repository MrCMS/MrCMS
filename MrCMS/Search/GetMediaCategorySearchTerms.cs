using System.Collections.Generic;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.Search
{
    public class GetMediaCategorySearchTerms : IGetMediaCategorySearchTerms
    {
        public IEnumerable<string> Get(MediaCategory mediaCategory)
        {
            yield return mediaCategory.Name;
        }
    }
}