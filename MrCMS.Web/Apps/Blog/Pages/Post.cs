using System.ComponentModel;
using System.Web.Mvc;
using System.Xml;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Application.Pages;

namespace MrCMS.Web.Apps.Blog.Pages
{
    public class Post : Webpage, IContainerItem
    {
        [DisplayName("Featured Image")]
        public virtual string FeatureImage { get; set; }

        public override void AddCustomSitemapData(UrlHelper urlHelper, XmlNode url, XmlDocument xmlDocument)
        {
            if (!string.IsNullOrEmpty(FeatureImage))
            {
                var image = url.AppendChild(xmlDocument.CreateElement("image:image"));
                var imageLoc = image.AppendChild(xmlDocument.CreateElement("image:loc"));
                imageLoc.InnerText = urlHelper.AbsoluteContent(FeatureImage);
            }
        }

        public virtual string ContainerUrl
        {
            get
            {
                var blog = Parent as Blog;
                if (blog != null) return "/" + blog.LiveUrlSegment;
                return "/";
            }
        }
    }
}