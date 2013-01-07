using System.Drawing;
using System.IO;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IImageProcessor
    {
        MediaFile GetImage(string imageUrl);

        void SetFileDimensions(MediaFile mediaFile, Stream stream);
        void SaveResizedImage(MediaFile file, Size size, byte[] fileBytes, string filePath);
    }
}