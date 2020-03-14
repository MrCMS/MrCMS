using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IGetLiveUrl
    {
        string GetUrlSegment(Webpage webpage, bool addLeadingSlash = false);
        Task<string> GetAbsoluteUrl(Webpage webpage);
    }
}