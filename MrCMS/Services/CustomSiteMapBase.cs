using System.Xml;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public abstract class CustomSiteMapBase
    {
        public abstract void AddCustomSiteMapData(Webpage webpage, XmlNode mainNode, XmlDocument document);
    }

    public abstract class CustomSiteMapBase<T> : CustomSiteMapBase where T : Webpage
    {
        public abstract void AddCustomSiteMapData(T webpage, XmlNode mainNode, XmlDocument document);

        public override void AddCustomSiteMapData(Webpage webpage, XmlNode mainNode, XmlDocument document)
        {
            AddCustomSiteMapData(webpage as T, mainNode, document);
        }
    }
}