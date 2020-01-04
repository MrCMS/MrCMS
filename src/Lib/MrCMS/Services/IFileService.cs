using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IFileService
    {
        Task<MediaFile> AddFile(Stream stream, string fileName, string contentType, long contentLength,
            MediaCategory mediaCategory);
        Task DeleteFile(MediaFile mediaFile);
        Task UpdateFile(MediaFile mediaFile);
        Task<string> GetFileLocation(MediaFile mediaFile, Size imageSize, bool getCdn = false);
        Task<string> GetFileLocation(Crop crop, Size imageSize, bool getCdn = false);
        Task<FilesPagedResult> GetFilesPaged(int? categoryId, bool imagesOnly, int page = 1);
        MediaFile GetFileByUrl(string url);
        MediaFile GetFile(string value);
        Task<string> GetFileUrl(MediaFile file, string value);
        void RemoveFolder(MediaCategory mediaCategory);
        void CreateFolder(MediaCategory mediaCategory);
        bool IsValidFileType(string fileName);
        Task DeleteFileSoft(MediaFile mediaFile);
    }
}