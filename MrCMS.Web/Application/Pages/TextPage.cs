using System.ComponentModel;
using System.Web.Mvc;
using System.Xml;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Web.Application.Pages
{
    //[DocumentTypeDefinition(ChildrenListType.BlackList, Name = "Text Page", IconClass = "icon-file", DisplayOrder = 1, Type = typeof(TextPage), WebGetAction = "View", WebGetController = "TextPage", DefaultLayoutName = "Three Column")]
    [MrCMSMapClass]
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