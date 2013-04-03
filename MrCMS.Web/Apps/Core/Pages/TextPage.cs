using System.ComponentModel;
using System.Web.Mvc;
using System.Xml;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Core.Pages
{
    public class TextPage : Webpage
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
    }
}