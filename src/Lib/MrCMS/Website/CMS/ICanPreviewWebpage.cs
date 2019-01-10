using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.CMS
{
    public interface ICanPreviewWebpage
    {
        bool CanPreview(Webpage webpage);
    }
}