using System.Drawing;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.Services
{
    public interface ICropService
    {
        Task<Crop> CreateCrop(MediaFile file, CropType cropType, Rectangle details);
    }
}