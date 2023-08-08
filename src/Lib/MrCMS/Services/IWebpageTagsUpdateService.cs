using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IWebpageTagsUpdateService
    {
        Task SetTags(string taglist, int id);
        Task SetTags(string taglist, Webpage webpage);
        Task SetTags(List<string> taglist, Webpage webpage);
    }
}