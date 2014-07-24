using System.Web.Mvc;
using System.Xml;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Services
{
    public class AddFeaturedImageToSitemap :CustomSiteMapBase<TextPage>
    {
        private readonly UrlHelper _urlHelper;

        public AddFeaturedImageToSitemap(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public override void AddCustomSiteMapData(TextPage webpage, XmlNode mainNode, XmlDocument document)
        {
            if (!string.IsNullOrEmpty(webpage.FeatureImage))
            {
                var image = mainNode.AppendChild(document.CreateElement("image:image"));
                var imageLoc = image.AppendChild(document.CreateElement("image:loc"));
                imageLoc.InnerText = _urlHelper.AbsoluteContent(webpage.FeatureImage);
            } 
        }
    }
}