using System;
using System.Drawing;
using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Services.Caching;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Helpers
{
    public static class ImageRenderingExtensions
    {
        //public static MvcHtmlString RenderImage(this HtmlHelper helper, string imageUrl, string alt = null, string title = null, object attributes = null)
        //{
        //    using (new SiteFilterDisabler(MrCMSApplication.Get<ISession>()))
        //    {
        //        if (String.IsNullOrWhiteSpace(imageUrl))
        //            return MvcHtmlString.Empty;

        //        var image = MrCMSApplication.Get<IImageProcessor>().GetImage(imageUrl);
        //        var tagBuilder = new TagBuilder("img");
        //        if (image == null)
        //            return MvcHtmlString.Empty;

        //        tagBuilder.Attributes.Add("src", imageUrl);
        //        tagBuilder.Attributes.Add("alt", alt ?? image.Description);
        //        tagBuilder.Attributes.Add("title", title ?? image.Title);
        //        if (attributes != null)
        //        {
        //            var routeValueDictionary = MrCMSHtmlHelper.AnonymousObjectToHtmlAttributes(attributes);
        //            foreach (var kvp in routeValueDictionary)
        //            {
        //                tagBuilder.Attributes.Add(kvp.Key, kvp.Value.ToString());
        //            }
        //        }
        //        return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        //    }
        //}

        public static MvcHtmlString RenderImage(this HtmlHelper helper, string imageUrl, Size targetSize = default(Size), string alt = null, string title = null, object attributes = null)
        {
            return helper.GetCached(MrCMSApplication.Get<MediaSettings>().GetCachingInfo(imageUrl,targetSize,alt,title,attributes), html =>
            {
                using (new SiteFilterDisabler(MrCMSApplication.Get<ISession>()))
                {
                    if (string.IsNullOrWhiteSpace(imageUrl))
                        return MvcHtmlString.Empty;

                    var imageProcessor = MrCMSApplication.Get<IImageProcessor>();
                    var image = imageProcessor.GetImage(imageUrl);
                    if (image == null)
                        return MvcHtmlString.Empty;

                    if (targetSize != default(Size) && ImageProcessor.RequiresResize(image.Size, targetSize))
                    {
                        var fileService = MrCMSApplication.Get<IFileService>();
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
            });
        }
    }
}