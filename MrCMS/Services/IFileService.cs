using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Paging;

namespace MrCMS.Services
{
    public interface IFileService
    {
        ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, int contentLength, MediaCategory mediaCategory);
        ViewDataUploadFilesResult[] GetFiles(int mediaCategoryId);
        MediaFile GetFile(int id);
        string GetImageUrl(MediaFile file, Size size);
        void DeleteFile(int id);
        void SaveFile(MediaFile mediaFile);
        List<ImageSize> GetImageSizes();
        string GetFileLocation(MediaFile mediaFile, ImageSize imageSize);
        FilesPagedResult GetFilesPaged(int? categoryId, bool imagesOnly, int page = 1, int pageSize = 10);
        MediaFile GetFileByLocation(string value);
        MediaFile GetImage(string imageUrl);
    }
}