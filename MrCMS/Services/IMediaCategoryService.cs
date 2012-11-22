using MrCMS.Entities.Documents.Media;

namespace MrCMS.Services
{
    public interface IMediaCategoryService
    {
        void SaveCategory(MediaCategory mediaCategory);
        MediaCategory GetCategory(int id);
    }
}
