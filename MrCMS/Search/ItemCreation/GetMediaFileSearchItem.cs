using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Search.Models;

namespace MrCMS.Search.ItemCreation
{
    public class GetMediaFileSearchItem : GetUniversalSearchItemBase<MediaFile>
    {
        public override UniversalSearchItem GetSearchItem(MediaFile entity)
        {
            return new UniversalSearchItem
            {
                DisplayName = entity.FileName,
                Id = entity.Id,
                SearchTerms =
                    new[] {entity.FileName, entity.FileExtension, entity.FileUrl, entity.Title, entity.Description},
                SystemType = typeof (MediaFile).FullName,
                ActionUrl = "/admin/file/edit/" + entity.Id
            };
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<MediaFile> entities)
        {
            return entities.Select(GetSearchItem).ToHashSet();
        }
    }
}