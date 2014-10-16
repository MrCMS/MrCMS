using System.Collections.Generic;
using System.IO;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IFileAdminService
    {
        ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, long contentLength,
            MediaCategory mediaCategory);

        ViewDataUploadFilesResult[] GetFiles(MediaCategory mediaCategory);
        void DeleteFile(MediaFile mediaFile);
        void SaveFile(MediaFile mediaFile);
        bool IsValidFileType(string fileName);
        IList<MediaFile> GetFilesForFolder(MediaCategory category);
        void CreateFolder(MediaCategory category);
        void SetOrders(List<SortItem> items);
        IList<MediaCategory> GetSubFolders(MediaCategory folder);

        string MoveFolders(IEnumerable<MediaCategory> folders, MediaCategory parent = null);
        void MoveFiles(IEnumerable<MediaFile> files, MediaCategory parent = null);
        void DeleteFilesSoft(IEnumerable<MediaFile> files);
        void DeleteFilesHard(IEnumerable<MediaFile> files);
        void DeleteFoldersSoft(IEnumerable<MediaCategory> folders);
    }
}