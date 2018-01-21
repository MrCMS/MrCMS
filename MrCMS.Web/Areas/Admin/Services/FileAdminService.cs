using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class FileAdminService : IFileAdminService
    {
        private readonly IFileService _fileService;
        private readonly MediaSettings _mediaSettings;
        private readonly IGetDocumentsByParent<MediaCategory> _getDocumentsByParent;
        private readonly ISession _session;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IRepository<MediaCategory> _mediaCategoryRepository;

        public FileAdminService(IFileService fileService, ISession session,
            IStringResourceProvider stringResourceProvider, IRepository<MediaCategory> mediaCategoryRepository,
            MediaSettings mediaSettings, IGetDocumentsByParent<MediaCategory> getDocumentsByParent)
        {
            _fileService = fileService;
            _session = session;
            _stringResourceProvider = stringResourceProvider;
            _mediaCategoryRepository = mediaCategoryRepository;
            _mediaSettings = mediaSettings;
            _getDocumentsByParent = getDocumentsByParent;
        }

        public ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, long contentLength,
            MediaCategory mediaCategory)
        {
            MediaFile mediaFile = _fileService.AddFile(stream, fileName, contentType, contentLength, mediaCategory);
            return mediaFile.GetUploadFilesResult();
        }

        public void DeleteFile(MediaFile mediaFile)
        {
            _fileService.DeleteFile(mediaFile);
        }

        public void SaveFile(MediaFile mediaFile)
        {
            _fileService.SaveFile(mediaFile);
        }

        public bool IsValidFileType(string fileName)
        {
            return _fileService.IsValidFileType(fileName);
        }

        public IPagedList<MediaFile> GetFilesForFolder(MediaCategorySearchModel searchModel)
        {
            IQueryOver<MediaFile, MediaFile> query = _session.QueryOver<MediaFile>();
            query = searchModel.Id.HasValue
                ? query.Where(file => file.MediaCategory.Id == searchModel.Id)
                : query.Where(file => file.MediaCategory == null);
            if (!string.IsNullOrWhiteSpace(searchModel.SearchText))
            {
                query = query.Where(file =>
                    file.FileName.IsInsensitiveLike(searchModel.SearchText, MatchMode.Anywhere)
                    ||
                    file.Title.IsInsensitiveLike(searchModel.SearchText, MatchMode.Anywhere)
                    ||
                    file.Description.IsInsensitiveLike(searchModel.SearchText, MatchMode.Anywhere)
                    );
            }
            query = query.OrderBy(searchModel.SortBy);

            return query.Paged(searchModel.Page, _mediaSettings.MediaPageSize);
        }

        public List<ImageSortItem> GetFilesToSort(MediaCategory category = null)
        {
            IQueryOver<MediaFile, MediaFile> query = _session.QueryOver<MediaFile>();
            query = category != null
                ? query.Where(file => file.MediaCategory.Id == category.Id)
                : query.Where(file => file.MediaCategory == null);
            query = query.OrderBy(x => x.DisplayOrder).Asc;

            ImageSortItem item = null;
            return query.SelectList(builder =>
                {
                    builder.Select(file => file.FileName).WithAlias(() => item.Name);
                    builder.Select(file => file.Id).WithAlias(() => item.Id);
                    builder.Select(file => file.DisplayOrder).WithAlias(() => item.Order);
                    builder.Select(file => file.FileExtension).WithAlias(() => item.FileExtension);
                    builder.Select(file => file.FileUrl).WithAlias(() => item.ImageUrl);
                    return builder;
                }).TransformUsing(Transformers.AliasToBean<ImageSortItem>())
                .Cacheable()
                .List<ImageSortItem>().ToList();
        }



        public void SetOrders(List<SortItem> items)
        {
            _session.Transact(session => items.ForEach(item =>
            {
                var mediaFile = session.Get<MediaFile>(item.Id);
                mediaFile.DisplayOrder = item.Order;
                session.Update(mediaFile);
            }));
        }

        public IList<MediaCategory> GetSubFolders(MediaCategorySearchModel searchModel)
        {
            IQueryOver<MediaCategory, MediaCategory> queryOver =
                _session.QueryOver<MediaCategory>().Where(x => !x.HideInAdminNav);
            queryOver = searchModel.Id.HasValue
                ? queryOver.Where(x => x.Parent.Id == searchModel.Id)
                : queryOver.Where(x => x.Parent == null);
            if (!string.IsNullOrWhiteSpace(searchModel.SearchText))
            {
                queryOver =
                    queryOver.Where(
                        category => category.Name.IsInsensitiveLike(searchModel.SearchText, MatchMode.Anywhere));
            }

            queryOver = queryOver.OrderBy(searchModel.SortBy);

            return queryOver.Cacheable().List();
        }

        public string MoveFolders(IEnumerable<MediaCategory> folders, MediaCategory parent = null)
        {
            string message = string.Empty;
            if (folders != null)
            {
                _session.Transact(s => folders.ForEach(item =>
                {
                    var mediaFolder = s.Get<MediaCategory>(item.Id);
                    if (parent != null && mediaFolder.Id != parent.Id)
                    {
                        mediaFolder.Parent = parent;
                        s.Update(mediaFolder);
                    }
                    else if (parent == null)
                    {
                        mediaFolder.Parent = null;
                        s.Update(mediaFolder);
                    }
                    else
                    {
                        message = _stringResourceProvider.GetValue("Cannot move folder to the same folder");
                    }
                }));
            }
            return message;
        }

        public void MoveFiles(IEnumerable<MediaFile> files, MediaCategory parent = null)
        {
            if (files != null)
            {
                _session.Transact(session => files.ForEach(item =>
                {
                    var mediaFile = session.Get<MediaFile>(item.Id);
                    mediaFile.MediaCategory = parent;
                    session.Update(mediaFile);
                }));
            }
        }

        public void DeleteFoldersSoft(IEnumerable<MediaCategory> folders)
        {
            if (folders != null)
            {
                IEnumerable<MediaCategory> foldersRecursive = GetFoldersRecursive(folders);
                _mediaCategoryRepository.Transact(repository =>
                {
                    foreach (MediaCategory f in foldersRecursive)
                    {
                        var folder = repository.Get(f.Id);
                        List<MediaFile> files = folder.Files.ToList();
                        foreach (MediaFile file in files)
                            _fileService.DeleteFileSoft(file);

                        repository.Delete(folder);
                    }
                });
            }
        }

        public MediaCategory GetCategory(MediaCategorySearchModel searchModel)
        {
            return searchModel.Id.HasValue
                ? _session.Get<MediaCategory>(searchModel.Id.Value)
                : null;
        }

        public List<SelectListItem> GetSortByOptions(MediaCategorySearchModel searchModel)
        {
            return EnumHelper<MediaCategorySortMethod>.GetOptions();
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