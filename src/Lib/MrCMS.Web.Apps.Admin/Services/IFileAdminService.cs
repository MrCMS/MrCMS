using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IFileAdminService
    {
        Task<ViewDataUploadFilesResult> AddFile(Stream stream, string fileName, string contentType, long contentLength,
            int mediaCategoryId);

        void DeleteFile(MediaFile mediaFile);
        void UpdateFile(MediaFile mediaFile);
        bool IsValidFileType(string fileName);
        IPagedList<MediaFile> GetFilesForFolder(MediaCategorySearchModel searchModel);
        List<ImageSortItem> GetFilesToSort(MediaCategory category = null);
        Task SetOrders(List<SortItem> items);
        IList<MediaCategory> GetSubFolders(MediaCategorySearchModel searchModel);

        Task<string> MoveFolders(IEnumerable<MediaCategory> folders, MediaCategory parent = null);
        Task MoveFiles(IEnumerable<MediaFile> files, MediaCategory parent = null);
        void DeleteFilesSoft(IEnumerable<MediaFile> files);
        void DeleteFilesHard(IEnumerable<MediaFile> files);
        Task DeleteFoldersSoft(IEnumerable<MediaCategory> folders);
        MediaCategory GetCategory(MediaCategorySearchModel searchModel);
        List<SelectListItem> GetSortByOptions(MediaCategorySearchModel searchModel);
        MediaFile GetFile(int id);
    }
}