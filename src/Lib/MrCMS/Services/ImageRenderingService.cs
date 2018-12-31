using System.Drawing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services.Caching;
using MrCMS.Settings;
using MrCMS.Website.Caching;
using NHibernate;

namespace MrCMS.Services
{
    public class ImageRenderingService : IImageRenderingService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IFileService _fileService;
        private readonly IImageProcessor _imageProcessor;
        private readonly MediaSettings _mediaSettings;
        private readonly ISession _session;

        public ImageRenderingService(ISession session, IImageProcessor imageProcessor, IFileService fileService,
            MediaSettings mediaSettings, ICacheManager cacheManager)
        {
            _session = session;
            _imageProcessor = imageProcessor;
            _fileService = fileService;
            _mediaSettings = mediaSettings;
            _cacheManager = cacheManager;
        }

        public ImageInfo GetImageInfo(string imageUrl, Size targetSize)
        {
            var crop = _imageProcessor.GetCrop(imageUrl);
            if (crop != null)
                return new ImageInfo
                {
                    Title = crop.Title,
                    Description = crop.Description,
                    ImageUrl = GetCropImageUrl(crop, targetSize)
                };
            var image = _imageProcessor.GetImage(imageUrl);
            if (image != null)
                return new ImageInfo
                {
                    Title = image.Title,
                    Description = image.Description,
                    ImageUrl = GetFileImageUrl(image, targetSize)
                };
            return null;
        }


        public string GetImageUrl(string imageUrl, Size targetSize)
        {
            var info = _mediaSettings.GetImageUrlCachingInfo(imageUrl, targetSize);
            return _cacheManager.Get(info.CacheKey, () => string.IsNullOrWhiteSpace(imageUrl)
                ? null
                : GetImageInfo(imageUrl, targetSize)?.ImageUrl, info.TimeToCache, info.ExpiryType);
        }

        public IHtmlContent RenderImage(IHtmlHelper helper, string imageUrl, Size targetSize = new Size(),
            string alt = null,
            string title = null, object attributes = null)
        {
            var cachingInfo = _mediaSettings.GetImageTagCachingInfo(imageUrl, targetSize, alt, title, attributes);
            return helper.GetCached(cachingInfo, htmlHelper =>
            {
                using (new SiteFilterDisabler(_session))
                {
                    if (string.IsNullOrWhiteSpace(imageUrl))
                        return HtmlString.Empty;

                    var imageInfo = GetImageInfo(imageUrl, targetSize);
                    if (imageInfo == null)
                        return HtmlString.Empty;

                    return ReturnTag(imageInfo, alt, title, attributes);
                }
            });
        }

        private string GetFileImageUrl(MediaFile image, Size targetSize)
        {
            return _fileService.GetFileLocation(image, targetSize, true);
        }

        private string GetCropImageUrl(Crop crop, Size targetSize)
        {
            return _fileService.GetFileLocation(crop, targetSize, true);
        }


        private IHtmlContent ReturnTag(ImageInfo imageInfo, string alt, string title, object attributes)
        {
            var tagBuilder = new TagBuilder("img");
            tagBuilder.Attributes.Add("src", imageInfo.ImageUrl);
            tagBuilder.Attributes.Add("alt", alt ?? imageInfo.Title);
            tagBuilder.Attributes.Add("title", title ?? imageInfo.Description);
            if (attributes != null)
            {
                var routeValueDictionary = MrCMSHtmlHelperExtensions.AnonymousObjectToHtmlAttributes(attributes);
                foreach (var kvp in routeValueDictionary) tagBuilder.Attributes.Add(kvp.Key, kvp.Value.ToString());
            }

            tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;
            return tagBuilder;
        }
    }
}