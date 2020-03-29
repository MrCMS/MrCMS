using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ILocalisedScripts
    {
        Task<IEnumerable<string>> GetFiles();
    }
}