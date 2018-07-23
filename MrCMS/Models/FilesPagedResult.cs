using System.Collections.Generic;
using MrCMS.Entities.Documents.Media;
using X.PagedList;

namespace MrCMS.Models
{
    public class FilesPagedResult : StaticPagedList<MediaFile>
    {
        public FilesPagedResult(IEnumerable<MediaFile> subset, IPagedList metaData, int? categoryId, bool imagesOnly)
            : base(subset, metaData)
        {
            CategoryId = categoryId;
            ImagesOnly = imagesOnly;
        }

        public int? CategoryId { get; set; }

        public bool ImagesOnly { get; set; }
    }
}