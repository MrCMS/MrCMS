using System.Globalization;
using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Services.Resources
{
    public interface IGetUserCultureInfo
    {
        Task<CultureInfo> Get(User user);
        Task<string> GetInfoString(User user);
    }
}