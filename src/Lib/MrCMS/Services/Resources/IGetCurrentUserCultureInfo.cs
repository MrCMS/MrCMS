using System.Globalization;
using System.Threading.Tasks;

namespace MrCMS.Services.Resources
{
    public interface IGetCurrentUserCultureInfo
    {
        Task<CultureInfo> Get();
        Task<string> GetInfoString();
    }
}