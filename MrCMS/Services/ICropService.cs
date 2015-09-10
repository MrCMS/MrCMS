using System.Drawing;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.Services
{
    public interface ICropService
    {
        Crop CreateCrop(MediaFile file, CropType cropType, Rectangle details);
    }
}