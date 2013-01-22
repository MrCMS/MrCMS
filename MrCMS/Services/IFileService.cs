using System.Drawing;
using System.IO;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IFileService
    {
        ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, int contentLength, MediaCategory mediaCategory);
        ViewDataUploadFilesResult[] GetFiles(MediaCategory mediaCategory);
        MediaFile GetFile(int id);
        void DeleteFile(MediaFile mediaFile);
        void SaveFile(MediaFile mediaFile);
        string GetFileLocation(MediaFile mediaFile, Size imageSize);
        FilesPagedResult GetFilesPaged(int? categoryId, bool imagesOnly, int page = 1, int pageSize = 10);
        MediaFile GetFileByLocation(string value);
        string GetFileUrl(string value);
        string MediaDirectory { get; }
        void RemoveFolder(MediaCategory mediaCategory);
        void CreateFolder(MediaCategory mediaCategory);
    }
}