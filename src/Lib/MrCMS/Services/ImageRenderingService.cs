using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Models;
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

        public async Task<ImageInfo> GetImageInfo(string imageUrl, Size targetSize)
        {
            var crop = await _imageProcessor.GetCrop(imageUrl);
            if (crop != null)
                return new ImageInfo
                {
                    Title = crop.Title,
                    Description = crop.Description,
                    ImageUrl = await GetCropImageUrl(crop, targetSize),
                    ActualSize = ImageProcessor.CalculateDimensions(crop.Size, targetSize)
                };
            var image = await _imageProcessor.GetImage(imageUrl);
            if (image != null)
                return new ImageInfo
                {
                    Title = image.Title,
                    Description = image.Description,
                    ImageUrl = await GetFileImageUrl(image, targetSize),
                    ActualSize = ImageProcessor.CalculateDimensions(image.Size, targetSize)
                };
            return null;
        }

        public async Task<string> GetImageUrl(string imageUrl, Size targetSize)
        {
            var info = _mediaSettings.GetImageUrlCachingInfo(imageUrl, targetSize);
            return await _cacheManager.GetOrCreateAsync(info.CacheKey, async () =>
            {
                var result = await GetImageInfo(imageUrl, targetSize);
                return result.ImageUrl;
            }, info.TimeToCache, info.ExpiryType);
        }

        public async Task<IHtmlContent> RenderImage(IHtmlHelper helper, string imageUrl, Size targetSize = new Size(),
            string alt = null,
            string title = null, bool enableCaption = false, object attributes = null)
        {
            var cachingInfo = _mediaSettings.GetImageTagCachingInfo(imageUrl, targetSize, alt, title, enableCaption, attributes);

            return await _cacheManager.GetOrCreateAsync(cachingInfo.CacheKey, async () =>
            {
                using (new SiteFilterDisabler(_session))
                {
                    if (string.IsNullOrWhiteSpace(imageUrl))
                        return HtmlString.Empty;

                    var imageInfo = await GetImageInfo(imageUrl, targetSize);
                    if (imageInfo == null)
                        return HtmlString.Empty;

                    return ReturnTag(imageInfo, alt, title, enableCaption, attributes);
                }
            }, cachingInfo.TimeToCache, cachingInfo.ExpiryType);
        }

        private async Task<string> GetFileImageUrl(MediaFile image, Size targetSize)
        {
            return await _fileService.GetFileLocation(image, targetSize, true);
        }

        private async Task<string> GetCropImageUrl(Crop crop, Size targetSize)
        {
            return await _fileService.GetFileLocation(crop, targetSize, true);
        }


        private IHtmlContent ReturnTag(ImageInfo imageInfo, string alt, string title, bool enableCaption, object attributes)
        {
            var tagBuilder = new TagBuilder("img");
            tagBuilder.Attributes.Add("src", imageInfo.ImageUrl);
            tagBuilder.Attributes.Add("alt", alt ?? imageInfo.Title);
            tagBuilder.Attributes.Add("title", title ?? imageInfo.Description);
            tagBuilder.Attributes.Add("loading", "lazy");
            if (attributes != null)
            {
                var routeValueDictionary = MrCMSHtmlHelperExtensions.AnonymousObjectToHtmlAttributes(attributes);
                foreach (var kvp in routeValueDictionary.Where(kvp => kvp.Value != null))
                    tagBuilder.Attributes.Add(kvp.Key, kvp.Value!.ToString());
            }

            tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;

            if (enableCaption && !string.IsNullOrWhiteSpace(imageInfo.Description))
            {
                var figureBuilder = new TagBuilder("figure");
                figureBuilder.Attributes.Add("class", "figure");
                var figCaptionBuilder = new TagBuilder("figCaption");
                figCaptionBuilder.Attributes.Add("class", "figure-caption py-2");
                figCaptionBuilder.InnerHtml.Append(imageInfo.Description);

                figureBuilder.InnerHtml.AppendHtml(tagBuilder);
                figureBuilder.InnerHtml.AppendHtml(figCaptionBuilder);

                return figureBuilder;
            }

            return tagBuilder;
        }
    }
}