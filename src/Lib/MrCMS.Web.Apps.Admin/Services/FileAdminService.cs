using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Settings;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Models;

using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class FileAdminService : IFileAdminService
    {
        private readonly IFileService _fileService;
        private readonly IRepository<MediaFile> _mediaFileRepository;
        private readonly MediaSettings _mediaSettings;
        private readonly IGetDocumentsByParent<MediaCategory> _getDocumentsByParent;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IRepository<MediaCategory> _mediaCategoryRepository;

        public FileAdminService(IFileService fileService, IRepository<MediaFile> mediaFileRepository,
            IStringResourceProvider stringResourceProvider, IRepository<MediaCategory> mediaCategoryRepository,
            MediaSettings mediaSettings, IGetDocumentsByParent<MediaCategory> getDocumentsByParent)
        {
            _fileService = fileService;
            _mediaFileRepository = mediaFileRepository;
            _stringResourceProvider = stringResourceProvider;
            _mediaCategoryRepository = mediaCategoryRepository;
            _mediaSettings = mediaSettings;
            _getDocumentsByParent = getDocumentsByParent;
        }

        public async Task<ViewDataUploadFilesResult> AddFile(Stream stream, string fileName, string contentType,
            long contentLength,
            int mediaCategoryId)
        {
            MediaFile mediaFile = await _fileService.AddFile(stream, fileName, contentType, contentLength, await _mediaCategoryRepository.Load(mediaCategoryId));
            return mediaFile.GetUploadFilesResult();
        }

        public void DeleteFile(MediaFile mediaFile)
        {
            _fileService.DeleteFile(mediaFile);
        }

        public void UpdateFile(MediaFile mediaFile)
        {
            _fileService.UpdateFile(mediaFile);
        }

        public bool IsValidFileType(string fileName)
        {
            return _fileService.IsValidFileType(fileName);
        }

        public IPagedList<MediaFile> GetFilesForFolder(MediaCategorySearchModel searchModel)
        {
            var query = _mediaFileRepository.Readonly();
            query = searchModel.Id.HasValue
                ? query.Where(file => file.MediaCategory.Id == searchModel.Id)
                : query.Where(file => file.MediaCategory == null);
            if (!string.IsNullOrWhiteSpace(searchModel.SearchText))
            {
                query = query.Where(file =>
                    EF.Functions.Like(file.FileName, $"%{searchModel.SearchText}%")
                    ||
                    EF.Functions.Like(file.Title, $"%{searchModel.SearchText}%")
                    ||
                    EF.Functions.Like(file.Description, $"%{searchModel.SearchText}%")
                    );
            }
            query = query.OrderBy(searchModel.SortBy);

            return PagedListExtensions.ToPagedList(query, searchModel.Page, _mediaSettings.MediaPageSize);
        }

        public List<ImageSortItem> GetFilesToSort(MediaCategory category = null)
        {
            var query = _mediaFileRepository.Readonly();
            query = category != null
                ? query.Where(file => file.MediaCategory.Id == category.Id)
                : query.Where(file => file.MediaCategory == null);
            query = query.OrderBy(x => x.DisplayOrder);

            return query.Select(mediaFile =>
                new ImageSortItem
                {
                    Id = mediaFile.Id,
                    Name = mediaFile.FileName,
                    FileExtension = mediaFile.FileExtension,
                    ImageUrl = mediaFile.FileUrl,
                    Order = mediaFile.DisplayOrder
                }).ToList();
        }



        public async Task SetOrders(List<SortItem> items)
        {
            var mediaFiles = items.Select(item =>
            {
                var mediaFile = _mediaFileRepository.LoadSync(item.Id);
                mediaFile.DisplayOrder = item.Order;
                return mediaFile;
            });
            await _mediaFileRepository.UpdateRange(mediaFiles.ToList());
        }

        public IList<MediaCategory> GetSubFolders(MediaCategorySearchModel searchModel)
        {
            var queryOver =
                _mediaCategoryRepository.Query().Where(x => !x.HideInAdminNav);
            queryOver = searchModel.Id.HasValue
                ? queryOver.Where(x => x.Parent.Id == searchModel.Id)
                : queryOver.Where(x => x.Parent == null);
            if (!string.IsNullOrWhiteSpace(searchModel.SearchText))
            {
                queryOver =
                    queryOver.Where(
                        category => EF.Functions.Like(category.Name, $"%{searchModel.SearchText}%"));
            }

            queryOver = queryOver.OrderBy(searchModel.SortBy);

            return queryOver.ToList();
        }

        public async Task<string> MoveFolders(IEnumerable<MediaCategory> folders, MediaCategory parent = null)
        {
            string message = string.Empty;
            if (folders != null)
            {
                folders.ForEach(item =>
                {
                    //var mediaFolder = _mediaCategoryRepository.LoadSync(item.Id);
                    if (parent != null && item.Id != parent.Id)
                    {
                        item.Parent = parent;
                    }
                    else if (parent == null)
                    {
                        item.Parent = null;
                    }
                    else
                    {
                        message = _stringResourceProvider.GetValue("Cannot move folder to the same folder");
                    }
                });
                await _mediaCategoryRepository.UpdateRange(folders.ToList());
            }
            return message;
        }

        public async Task MoveFiles(IEnumerable<MediaFile> files, MediaCategory parent = null)
        {
            if (files != null)
            {
                files.ForEach(item =>
                {
                    item.MediaCategory = parent;
                });
                await _mediaFileRepository.UpdateRange(files.ToList());
            }
        }

        public async Task DeleteFoldersSoft(IEnumerable<MediaCategory> folders)
        {
            if (folders != null)
            {
                IEnumerable<MediaCategory> foldersRecursive = GetFoldersRecursive(folders);
                await _mediaCategoryRepository.Transact(async (repo, ct) =>
                 {
                     foreach (MediaCategory f in foldersRecursive)
                     {
                         var folder = repo.LoadSync(f.Id);
                         List<MediaFile> files = folder.Files.ToList();
                         foreach (MediaFile file in files)
                             await _fileService.DeleteFileSoft(file);

                         await repo.Delete(folder, ct);
                     }
                 });
            }
        }

        public MediaCategory GetCategory(MediaCategorySearchModel searchModel)
        {
            return searchModel.Id.HasValue
                ? _mediaCategoryRepository.LoadSync(searchModel.Id.Value)
                : null;
        }

        public List<SelectListItem> GetSortByOptions(MediaCategorySearchModel searchModel)
        {
            return EnumHelper<MediaCategorySortMethod>.GetOptions();
        }

        public MediaFile GetFile(int id)
        {
            return _mediaFileRepository.LoadSync(id);
        }

        public void DeleteFilesSoft(IEnumerable<MediaFile> files)
        {
            if (files != null)
            {
                foreach (MediaFile file in files)
                    _fileService.DeleteFileSoft(file);
            }
        }

        public void DeleteFilesHard(IEnumerable<MediaFile> files)
        {
            if (files != null)
            {
                foreach (MediaFile file in files)
                    _fileService.DeleteFile(file);
            }
        }

        private IEnumerable<MediaCategory> GetFoldersRecursive(IEnumerable<MediaCategory> categories)
        {
            foreach (MediaCategory category in categories)
            {
                foreach (MediaCategory child in GetFoldersRecursive(_getDocumentsByParent.GetDocuments(category)))
                {
                    yield return child;
                }
                yield return category;
            }
        }
    }
}