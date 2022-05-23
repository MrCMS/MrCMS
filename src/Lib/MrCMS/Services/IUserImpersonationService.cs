using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Services;

public interface IUserImpersonationService
{
    Task<UserImpersonationResult> Impersonate(User user);
    void CancelImpersonation();
}