using System.Xml;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface ICustomSiteMapService
    {
        void AddCustomSiteMapData(Webpage webpage, XmlNode mainNode, XmlDocument document);
    }
}