using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public interface ISetVerifiedUserData
    {
        Task SetUserData(User user);
    }
}