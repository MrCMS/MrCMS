using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Events
{
    public class SetUpdatedUser : IOnUpdating<SystemEntity>
    {
        private readonly IGetCurrentClaimsPrincipal _getCurrentClaimsPrincipal;

        public SetUpdatedUser(IGetCurrentClaimsPrincipal getCurrentClaimsPrincipal)
        {
            _getCurrentClaimsPrincipal = getCurrentClaimsPrincipal;
        }

        public async Task Execute(OnUpdatingArgs<SystemEntity> args)
        {
            var user = await _getCurrentClaimsPrincipal.GetPrincipal();
            if (user != null)
                args.Item.UpdatedBy = await args.Session.LoadAsync<User>(user.GetUserId());
        }
    }
}