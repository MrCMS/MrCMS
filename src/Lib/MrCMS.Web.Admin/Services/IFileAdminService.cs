using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IFileAdminService
    {
        ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, long contentLength,
            int mediaCategoryId);

        void DeleteFile(MediaFile mediaFile);
        MediaFile UpdateSEO(UpdateFileSEOModel model);
        bool IsValidFileType(string fileName);
        IPagedList<MediaFile> GetFilesForFolder(MediaCategorySearchModel searchModel);
        List<ImageSortItem> GetFilesToSort(MediaCategory category = null);
        void SetOrders(List<SortItem> items);
        IList<MediaCategory> GetSubFolders(MediaCategorySearchModel searchModel);
        UpdateFileSEOModel GetEditModel(int id);

        string MoveFolders(IEnumerable<MediaCategory> folders, MediaCategory parent = null);
        void MoveFiles(IEnumerable<MediaFile> files, MediaCategory parent = null);
        void DeleteFilesSoft(IEnumerable<MediaFile> files);
        void DeleteFilesHard(IEnumerable<MediaFile> files);
        void DeleteFoldersSoft(IEnumerable<MediaCategory> folders);
        MediaCategory GetCategory(MediaCategorySearchModel searchModel);
        List<SelectListItem> GetSortByOptions(MediaCategorySearchModel searchModel);
        MediaFile GetFile(int id);
    }
}