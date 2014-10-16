using System.Collections.Generic;
using System.IO;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class FileAdminService : IFileAdminService
    {
        private readonly IFileService _fileService;
        private readonly ISession _session;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IDocumentService _documentService;

        public FileAdminService(IFileService fileService, ISession session, IStringResourceProvider stringResourceProvider, IDocumentService documentService)
        {
            _fileService = fileService;
            _session = session;
            _stringResourceProvider = stringResourceProvider;
            _documentService = documentService;
        }

        public ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, long contentLength, MediaCategory mediaCategory)
        {
            var mediaFile = _fileService.AddFile(stream, fileName, contentType, contentLength, mediaCategory);
            return mediaFile.GetUploadFilesResult();
        }

        public ViewDataUploadFilesResult[] GetFiles(MediaCategory mediaCategory = null)
        {
            return
                _session.QueryOver<MediaFile>()
                    .Where(file => file.MediaCategory == mediaCategory)
                    .OrderBy(file => file.DisplayOrder)
                    .Asc.Cacheable()
                    .List()
                    .Select(file => FileHelper.GetUploadFilesResult(file)).ToArray();
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
        public IList<MediaFile> GetFilesForFolder(MediaCategory category = null)
        {
            var query = _session.QueryOver<MediaFile>();
            query = category != null
                ? query.Where(file => file.MediaCategory.Id == category.Id)
                : query.Where(file => file.MediaCategory == null);

            return query.OrderBy(x => x.DisplayOrder).Desc.Cacheable().List();
        }

        public void CreateFolder(MediaCategory category)
        {
            _fileService.CreateFolder(category);
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

        public IList<MediaCategory> GetSubFolders(MediaCategory folder = null)
        {
            IQueryOver<MediaCategory, MediaCategory> queryOver = _session.QueryOver<MediaCategory>().Where(x=>!x.HideInAdminNav);
            if (folder != null && folder.Id > 0)
                return queryOver.Where(x => x.Parent == folder).Cacheable().List();
            return queryOver.Where(x => x.Parent == null).Cacheable().List();
        }

        public string MoveFolders(IEnumerable<MediaCategory> folders, MediaCategory parent = null)
        {
            var message = string.Empty;
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
                foreach (var f in foldersRecursive)
                {
                    var folder = _documentService.GetDocument<MediaCategory>(f.Id);
                    var files = folder.Files.ToList();
                    foreach (var file in files)
                        _fileService.DeleteFileSoft(file);

                    _documentService.DeleteDocument(folder);
                }
            }
        }

        private IEnumerable<MediaCategory> GetFoldersRecursive(IEnumerable<MediaCategory> categories)
        {
            foreach (var category in categories)
            {
                foreach (var child in GetFoldersRecursive(_documentService.GetDocumentsByParent<MediaCategory>(category)))
                {
                    yield return child;
                }
                yield return category;
            }
        }

        public void DeleteFilesSoft(IEnumerable<MediaFile> files)
        {
            if (files != null)
            {
                foreach (var file in files)
                    _fileService.DeleteFileSoft(file);
            }
        }

        public void DeleteFilesHard(IEnumerable<MediaFile> files)
        {
            if (files != null)
            {
                foreach (var file in files)
                    _fileService.DeleteFile(file);
            }
        }

    }
}