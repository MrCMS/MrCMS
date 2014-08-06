using System.Drawing;
using System.Web.Mvc;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Services.Caching;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services
{
    public class ImageTagRenderingService : IImageTagRenderingService
    {
        private readonly ISession _session;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileService _fileService;
        private readonly MediaSettings _mediaSettings;

        public ImageTagRenderingService(ISession session, IImageProcessor imageProcessor, IFileService fileService, MediaSettings mediaSettings)
        {
            _session = session;
            _imageProcessor = imageProcessor;
            _fileService = fileService;
            _mediaSettings = mediaSettings;
        }

        public MvcHtmlString RenderImage(HtmlHelper helper, string imageUrl, Size targetSize = new Size(), string alt = null,
            string title = null, object attributes = null)
        {
            var cachingInfo = _mediaSettings.GetCachingInfo(imageUrl, targetSize, alt, title, attributes);
            return helper.GetCached(cachingInfo, htmlHelper =>
               {
                   using (new SiteFilterDisabler(_session))
                   {
                       if (string.IsNullOrWhiteSpace(imageUrl))
                           return MvcHtmlString.Empty;

                       var image = _imageProcessor.GetImage(imageUrl);
                       if (image == null)
                           return MvcHtmlString.Empty;

                       if (targetSize != default(Size) && ImageProcessor.RequiresResize(image.Size, targetSize))
                       {
                           var location = _fileService.GetFileLocation(image, targetSize);
                           if (!string.IsNullOrWhiteSpace(location))
                               imageUrl = location;
                       }

                       var tagBuilder = new TagBuilder("img");
                       tagBuilder.Attributes.Add("src", imageUrl);
                       tagBuilder.Attributes.Add("alt", alt ?? image.Title);
                       tagBuilder.Attributes.Add("title", title ?? image.Description);
                       if (attributes != null)
                       {
                           var routeValueDictionary = MrCMSHtmlHelper.AnonymousObjectToHtmlAttributes(attributes);
                           foreach (var kvp in routeValueDictionary)
                           {
                               tagBuilder.Attributes.Add(kvp.Key, kvp.Value.ToString());
                           }
                       }
                       return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
                   }
               });
        }
    }
}