using System.Drawing;
using System.IO;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public interface IImageProcessor
    {
        MediaFile GetImage(string imageUrl);

        void SetFileDimensions(MediaFile file, Stream stream);
        void SaveResizedImage(MediaFile file, Size size, byte[] fileBytes, string filePath);
        void EnforceMaxSize(ref Stream stream, MediaFile file, MediaSettings mediaSettings);
    }
}