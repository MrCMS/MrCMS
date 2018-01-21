using Microsoft.AspNet.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserStore : IUserLoginStore<User, int>, IUserClaimStore<User, int>, IUserRoleStore<User, int>
    {
    }
}