using Microsoft.AspNet.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class UserManager : UserManager<User,int>, IUserManager
    {
        public UserManager(IUserStore store) : base(store)
        {
        }
    }
}