using System;
using System.Drawing;
using System.Web.Mvc;
using MrCMS.DbConfiguration;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Helpers
{
    public static class ImageRenderingExtensions
    {
        public static MvcHtmlString RenderImage(this HtmlHelper helper, string imageUrl, string alt = null, string title = null, object attributes = null)
        {
            using (new SiteFilterDisabler(MrCMSApplication.Get<ISession>()))
            {
                if (String.IsNullOrWhiteSpace(imageUrl))
                    return MvcHtmlString.Empty;

                var image = MrCMSApplication.Get<IImageProcessor>().GetImage(imageUrl);
                var tagBuilder = new TagBuilder("img");
                if (image == null)
                    return MvcHtmlString.Empty;

                tagBuilder.Attributes.Add("src", imageUrl);
                tagBuilder.Attributes.Add("alt", alt ?? image.Description);
                tagBuilder.Attributes.Add("title", title ?? image.Title);
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
        }

        public static MvcHtmlString RenderImage(this HtmlHelper helper, string imageUrl, Size targetSize, string alt = null, string title = null, object attributes = null)
        {
            using (new SiteFilterDisabler(MrCMSApplication.Get<ISession>()))
            {
                if (String.IsNullOrWhiteSpace(imageUrl))
                    return MvcHtmlString.Empty;

                var imageProcessor = MrCMSApplication.Get<IImageProcessor>();
                var fileService = MrCMSApplication.Get<IFileService>();
                var image = imageProcessor.GetImage(imageUrl);

                if (image == null || targetSize == Size.Empty)
                    return MvcHtmlString.Empty;

                if (ImageProcessor.RequiresResize(image.Size, targetSize))
                {
                    var location = fileService.GetFileLocation(image, targetSize);
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
        }
    }
}