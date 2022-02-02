using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.Services;

public interface IGetMediaCategoriesByParent
{
    Task<IReadOnlyList<MediaCategory>> GetMediaCategories(MediaCategory parent);
}