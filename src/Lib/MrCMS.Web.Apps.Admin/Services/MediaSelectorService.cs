using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models;

using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class MediaSelectorService : IMediaSelectorService
    {
        private readonly IRepository<MediaFile> _mediaFileRepository;
        private readonly IRepository<MediaCategory> _mediaCategoryRepository;
        private readonly IFileService _fileService;
        private readonly IImageProcessor _imageProcessor;

        public MediaSelectorService(IRepository<MediaFile> mediaFileRepository, IRepository<MediaCategory> mediaCategoryRepository, IFileService fileService, IImageProcessor imageProcessor)
        {
            _mediaFileRepository = mediaFileRepository;
            _mediaCategoryRepository = mediaCategoryRepository;
            _fileService = fileService;
            _imageProcessor = imageProcessor;
        }

        public IPagedList<MediaFile> Search(MediaSelectorSearchQuery searchQuery)
        {
            var queryOver = _mediaFileRepository.Query();
            if (searchQuery.CategoryId.HasValue)
                queryOver = queryOver.Where(file => file.MediaCategory.Id == searchQuery.CategoryId);
            if (!string.IsNullOrWhiteSpace(searchQuery.Query))
            {
                var term = searchQuery.Query.Trim();
                queryOver =
                    queryOver.Where(
                        file =>
                            EF.Functions.Like(file.FileName, $"%{term}%") ||
                            EF.Functions.Like(file.Title, $"%{term}%") ||
                            EF.Functions.Like(file.Description, $"%{term}%"));
            }
            return queryOver.OrderByDescending(file => file.CreatedOn).ToPagedList(searchQuery.Page);
        }

        public List<SelectListItem> GetCategories()
        {
            return _mediaCategoryRepository.Readonly()
                .Where(category => category.HideInAdminNav != true)
                .ToList()
                .BuildSelectItemList(category => category.Name, category => category.Id.ToString(),
                    emptyItemText: "All categories");
        }

        public async Task<SelectedItemInfo> GetFileInfo(string value)
        {
            var file = await _fileService.GetFile(value);
            if (file == null)
            {
                return null;
            }

            return new SelectedItemInfo
            {
                Url = await _fileService.GetFileUrl(file, value),
                Alt = file.Title,
                Description = file.Description,
            };
        }

        public async Task<string> GetAlt(string url)
        {
            var mediaFile = await GetImage(url);
            return mediaFile != null ? mediaFile.Title : string.Empty;
        }

        public async Task<string> GetDescription(string url)
        {
            var mediaFile = await GetImage(url);
            return mediaFile != null ? mediaFile.Description : string.Empty;
        }

        private async Task<MediaFile> GetImage(string url)
        {
            return await _imageProcessor.GetImage(url);
        }

        public async Task<bool> UpdateAlt(UpdateMediaParams updateMediaParams)
        {
            var mediaFile = await GetImage(updateMediaParams.Url);
            if (mediaFile == null)
                return false;
            mediaFile.Title = updateMediaParams.Value;
            await _mediaFileRepository.Update((mediaFile));
            return true;
        }

        public async Task<bool> UpdateDescription(UpdateMediaParams updateMediaParams)
        {
            var mediaFile = await GetImage(updateMediaParams.Url);
            if (mediaFile == null)
                return false;
            mediaFile.Description = updateMediaParams.Value;
            await _mediaFileRepository.Update((mediaFile));
            return true;
        }
    }
}