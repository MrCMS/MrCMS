using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IFileAdminService
    {
        ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, long contentLength, MediaCategory mediaCategory);
        ViewDataUploadFilesResult[] GetFiles(MediaCategory mediaCategory);
        void DeleteFile(MediaFile mediaFile);
        void SaveFile(MediaFile mediaFile);
        bool IsValidFileType(string fileName);
        IPagedList<MediaFile> GetFilesForSearchPaged(MediaCategorySearchModel model);
        void CreateFolder(MediaCategory category);
        void SetOrders(List<SortItem> items);
    }

    public class FileAdminService : IFileAdminService
    {
        private readonly IFileService _fileService;
        private readonly ISession _session;

        public FileAdminService(IFileService fileService, ISession session)
        {
            _fileService = fileService;
            _session = session;
        }

        public ViewDataUploadFilesResult AddFile(Stream stream, string fileName, string contentType, long contentLength, MediaCategory mediaCategory)
        {
            var mediaFile = _fileService.AddFile(stream, fileName, contentType, contentLength, mediaCategory);
            return mediaFile.GetUploadFilesResult();
        }

        public ViewDataUploadFilesResult[] GetFiles(MediaCategory mediaCategory)
        {
            return
                _session.QueryOver<MediaFile>()
                    .Where(file => file.MediaCategory == mediaCategory)
                    .OrderBy(file => file.DisplayOrder)
                    .Asc.Cacheable()
                    .List()
                    .Select(file => file.GetUploadFilesResult()).ToArray();
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
        public IPagedList<MediaFile> GetFilesForSearchPaged(MediaCategorySearchModel model)
        {
            var query = _session.QueryOver<MediaFile>();
            if (model.Id > 0)
                query = query.Where(x => x.MediaCategory.Id == model.Id);
            if (model.SearchText != null)
                query = query.Where(x => x.FileName.IsLike(model.SearchText, MatchMode.Anywhere) || x.Title.IsLike(model.SearchText, MatchMode.Anywhere) || x.Description.IsLike(model.SearchText, MatchMode.Anywhere));

            return query.OrderBy(x => x.DisplayOrder).Desc.Paged(model.Page);
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
    }
}