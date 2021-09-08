using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Services;

namespace MrCMS.Events
{
    public class SetCreatedUser : IOnAdding<SystemEntity>
    {
        private readonly IGetCurrentUser _getCurrentUser;

        public SetCreatedUser(IGetCurrentUser getCurrentUser)
        {
            _getCurrentUser = getCurrentUser;
        }
        public async Task Execute(OnAddingArgs<SystemEntity> args)
        {
            var user = await _getCurrentUser.Get();
            if (user != null)
                args.Item.CreatedBy = user;
        }
    }
}