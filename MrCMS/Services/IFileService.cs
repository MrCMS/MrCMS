using System.IO;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IFileService
    {
        ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, int contentLength, MediaCategory mediaCategory);
        ViewDataUploadFilesResult[] GetFiles(int mediaCategoryId);
        MediaFile GetFile(int id);
        void DeleteFile(MediaFile mediaFile);
        void SaveFile(MediaFile mediaFile);
        string GetFileLocation(MediaFile mediaFile, ImageSize imageSize);
        FilesPagedResult GetFilesPaged(int? categoryId, bool imagesOnly, int page = 1, int pageSize = 10);
        MediaFile GetFileByLocation(string value);
        string GetFileUrl(string value);
    }
}