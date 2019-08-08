using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IFileService
    {
        MediaFile AddFile(Stream stream, string fileName, string contentType, long contentLength,
            MediaCategory mediaCategory);
        void DeleteFile(MediaFile mediaFile);
        void SaveFile(MediaFile mediaFile);
        string GetFileLocation(MediaFile mediaFile, Size imageSize, bool getCdn = false);
        string GetFileLocation(Crop crop, Size imageSize, bool getCdn = false);
        FilesPagedResult GetFilesPaged(int? categoryId, bool imagesOnly, int page = 1);
        MediaFile GetFileByUrl(string url);
        MediaFile GetFile(string value);
        string GetFileUrl(MediaFile file, string value);
        void RemoveFolder(MediaCategory mediaCategory);
        void CreateFolder(MediaCategory mediaCategory);
        bool IsValidFileType(string fileName);
        void DeleteFileSoft(MediaFile mediaFile);
    }
}