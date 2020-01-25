using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.CMS
{
    public interface ICanPreviewWebpage
    {
        Task<bool> CanPreview(Webpage webpage);
    }
}