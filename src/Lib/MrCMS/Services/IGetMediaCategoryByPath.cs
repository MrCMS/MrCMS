using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.Services;

public interface IGetMediaCategoryByPath
{
    Task<MediaCategory> GetByPath(string path);
}