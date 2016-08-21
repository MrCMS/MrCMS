using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IFileAdminService
    {
        ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, long contentLength,
            MediaCategory mediaCategory);

        void DeleteFile(MediaFile mediaFile);
        void SaveFile(MediaFile mediaFile);
        bool IsValidFileType(string fileName);
        IPagedList<MediaFile> GetFilesForFolder(MediaCategorySearchModel searchModel);
        List<ImageSortItem> GetFilesToSort(MediaCategory category = null);
        void SetOrders(List<SortItem> items);
        IList<MediaCategory> GetSubFolders(MediaCategorySearchModel searchModel);

        string MoveFolders(IEnumerable<MediaCategory> folders, MediaCategory parent = null);
        void MoveFiles(IEnumerable<MediaFile> files, MediaCategory parent = null);
        void DeleteFilesSoft(IEnumerable<MediaFile> files);
        void DeleteFilesHard(IEnumerable<MediaFile> files);
        void DeleteFoldersSoft(IEnumerable<MediaCategory> folders);
        MediaCategory GetCategory(MediaCategorySearchModel searchModel);
        List<SelectListItem> GetSortByOptions(MediaCategorySearchModel searchModel);
    }
}