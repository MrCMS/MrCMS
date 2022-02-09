using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Services
{
    public interface IGetActivePages
    {
        Task<IList<string>> GetActiveUrls(int pageId);
    }
}