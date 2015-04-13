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
                PrimarySearchTerms = new[] {entity.FileName, entity.Title, entity.Description},
                SecondarySearchTerms = new[] {entity.FileExtension, entity.FileUrl},
                SystemType = typeof (MediaFile).FullName,
                ActionUrl = "/admin/file/edit/" + entity.Id,
                CreatedOn = entity.CreatedOn
            };
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<MediaFile> entities)
        {
            return entities.Select(GetSearchItem).ToHashSet();
        }
    }
}