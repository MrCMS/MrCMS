using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public interface IImageProcessor
    {
        Task<MediaFile> GetImage(string imageUrl);
        void SetFileDimensions(MediaFile mediaFile, Stream stream);
        Task SaveResizedImage(MediaFile file, Size size, byte[] fileBytes, string fileUrl);
        Task SaveResizedCrop(Crop crop, Size size, byte[] fileBytes, string fileUrl);
        Task SaveCrop(MediaFile file, CropType cropType, Rectangle cropInfo, byte[] fileBytes, string fileUrl);
        void EnforceMaxSize(ref Stream stream, MediaFile file, MediaSettings mediaSettings);
    }
}