using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Admin.Models;
using NHibernate;
using NHibernate.Criterion;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class MediaSelectorService : IMediaSelectorService
    {
        private readonly ISession _session;
        private readonly IFileService _fileService;
        private readonly IImageProcessor _imageProcessor;
        private readonly Site _site;

        public MediaSelectorService(ISession session, IFileService fileService, IImageProcessor imageProcessor, Site site)
        {
            _session = session;
            _fileService = fileService;
            _imageProcessor = imageProcessor;
            _site = site;
        }

        public IPagedList<MediaFile> Search(MediaSelectorSearchQuery searchQuery)
        {
            var queryOver = _session.QueryOver<MediaFile>().Where(file => file.Site.Id == _site.Id);
            if (searchQuery.CategoryId.HasValue)
                queryOver = queryOver.Where(file => file.MediaCategory.Id == searchQuery.CategoryId);
            if (!string.IsNullOrWhiteSpace(searchQuery.Query))
            {
                var term = searchQuery.Query.Trim();
                queryOver =
                    queryOver.Where(
                        file =>
                            file.FileName.IsLike(term, MatchMode.Anywhere) ||
                            file.Title.IsLike(term, MatchMode.Anywhere) ||
                            file.Description.IsLike(term, MatchMode.Anywhere));
            }
            return queryOver.OrderBy(file => file.CreatedOn).Desc.Paged(searchQuery.Page);
        }

        public List<SelectListItem> GetCategories()
        {
            return _session.QueryOver<MediaCategory>()
                .Where(category => category.Site.Id == _site.Id)
                .Where(category => category.HideInAdminNav != true)
                .Cacheable()
                .List()
                .BuildSelectItemList(category => category.Name, category => category.Id.ToString(),
                    emptyItemText: "All categories");
        }

        public SelectedItemInfo GetFileInfo(string value)
        {
            var file = _fileService.GetFile(value);
            if (file == null)
            {
                return null;
            }

            return new SelectedItemInfo
            {
                Url = _fileService.GetFileUrl(file,value),
                Alt = file.Title,
                Description = file.Description,
            };
        }

        public string GetAlt(string url)
        {
            var mediaFile = GetImage(url);
            return mediaFile != null ? mediaFile.Title : string.Empty;
        }

        public string GetDescription(string url)
        {
            var mediaFile = GetImage(url);
            return mediaFile != null ? mediaFile.Description : string.Empty;
        }

        private MediaFile GetImage(string url)
        {
            return _imageProcessor.GetImage(url);
        }

        public bool UpdateAlt(UpdateMediaParams updateMediaParams)
        {
            var mediaFile = GetImage(updateMediaParams.Url);
            if (mediaFile == null)
                return false;
            mediaFile.Title = updateMediaParams.Value;
            _session.Transact(session => session.Update(mediaFile));
            return true;
        }

        public bool UpdateDescription(UpdateMediaParams updateMediaParams)
        {
            var mediaFile = GetImage(updateMediaParams.Url);
            if (mediaFile == null)
                return false;
            mediaFile.Description = updateMediaParams.Value;
            _session.Transact(session => session.Update(mediaFile));
            return true;
        }
    }
}