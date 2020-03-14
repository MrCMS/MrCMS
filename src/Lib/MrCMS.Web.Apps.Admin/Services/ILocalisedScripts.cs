using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ILocalisedScripts
    {
        Task<IEnumerable<string>> GetFiles();
    }
}