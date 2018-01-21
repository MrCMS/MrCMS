using System;
using System.Drawing;
using System.Web.Mvc;
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
        private readonly ISession _session;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileService _fileService;
        private readonly MediaSettings _mediaSettings;
        private readonly ICacheManager _cacheManager;

        public ImageRenderingService(ISession session, IImageProcessor imageProcessor, IFileService fileService, MediaSettings mediaSettings, ICacheManager cacheManager)
        {
            _session = session;
            _imageProcessor = imageProcessor;
            _fileService = fileService;
            _mediaSettings = mediaSettings;
            _cacheManager = cacheManager;
        }

        public MvcHtmlString RenderImage(HtmlHelper helper, string imageUrl, Size targetSize = new Size(), string alt = null,
            string title = null, object attributes = null)
        {
            var cachingInfo = _mediaSettings.GetImageTagCachingInfo(imageUrl, targetSize, alt, title, attributes);
            return helper.GetCached(cachingInfo, htmlHelper =>
            {
                using (new SiteFilterDisabler(_session))
                {
                    if (string.IsNullOrWhiteSpace(imageUrl))
                        return MvcHtmlString.Empty;

                    var imageInfo = GetImageInfo(imageUrl, targetSize);
                    if (imageInfo == null)
                        return MvcHtmlString.Empty;

                    return ReturnTag(imageInfo, alt, title, attributes);
                }
            });
        }

        public ImageInfo GetImageInfo(string imageUrl, Size targetSize)
        {
            var crop = _imageProcessor.GetCrop(imageUrl);
            if (crop != null)
            {
                return new ImageInfo
                {
                    Title = crop.Title,
                    Description = crop.Description,
                    ImageUrl = GetCropImageUrl(crop, targetSize)
                };
            }
            MediaFile image = _imageProcessor.GetImage(imageUrl);
            if (image != null)
            {
                return new ImageInfo
                {
                    Title = image.Title,
                    Description = image.Description,
                    ImageUrl = GetFileImageUrl(image, targetSize)
                };
            }
            return null;
        }

        private string GetFileImageUrl(MediaFile image, Size targetSize)
        {
            return _fileService.GetFileLocation(image, targetSize, true);
        }

        private string GetCropImageUrl(Crop crop, Size targetSize)
        {
            return _fileService.GetFileLocation(crop, targetSize, true);
        }


        public string GetImageUrl(string imageUrl, Size targetSize)
        {
            var info = _mediaSettings.GetImageUrlCachingInfo(imageUrl, targetSize);
            return _cacheManager.Get(info.CacheKey, () => string.IsNullOrWhiteSpace(imageUrl)
                ? null
                : GetImageInfo(imageUrl, targetSize)?.ImageUrl, info.TimeToCache, info.ExpiryType);
        }


        private MvcHtmlString ReturnTag(ImageInfo imageInfo, string alt, string title, object attributes)
        {
            var tagBuilder = new TagBuilder("img");
            tagBuilder.Attributes.Add("src", imageInfo.ImageUrl);
            tagBuilder.Attributes.Add("alt", alt ?? imageInfo.Title);
            tagBuilder.Attributes.Add("title", title ?? imageInfo.Description);
            if (attributes != null)
            {
                var routeValueDictionary = MrCMSHtmlHelperExtensions.AnonymousObjectToHtmlAttributes(attributes);
                foreach (var kvp in routeValueDictionary)
                {
                    tagBuilder.Attributes.Add(kvp.Key, kvp.Value.ToString());
                }
            }
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

       
    }
}