using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Services;

namespace MrCMS.Events
{
    public class SetUpdatedUser : IOnUpdating<SystemEntity>
    {
        private readonly IGetCurrentUser _getCurrentUser;

        public SetUpdatedUser(IGetCurrentUser getCurrentUser)
        {
            _getCurrentUser = getCurrentUser;
        }
        public async Task Execute(OnUpdatingArgs<SystemEntity> args)
        {
            var user = await _getCurrentUser.Get();
            if (user != null)
                args.Item.UpdatedBy = args.Session.Get<User>(user.Id);
        }
    }
}
