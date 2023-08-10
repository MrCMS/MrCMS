using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Events
{
    public class SetCreatedUser : IOnAdding<SystemEntity>
    {
        private readonly IGetCurrentClaimsPrincipal _getCurrentClaimsPrincipal;

        public SetCreatedUser(IGetCurrentClaimsPrincipal getCurrentClaimsPrincipal)
        {
            _getCurrentClaimsPrincipal = getCurrentClaimsPrincipal;
        }

        public async Task Execute(OnAddingArgs<SystemEntity> args)
        {
            var user = await _getCurrentClaimsPrincipal.GetPrincipal();
            if (user != null)
                args.Item.CreatedBy = await args.Session.LoadAsync<User>(user.GetUserId());
        }
    }
}