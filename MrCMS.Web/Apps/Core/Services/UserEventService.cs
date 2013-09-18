using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Core.Services
{
    public class UserEventService : IUserEventService
    {
        private readonly IEnumerable<IOnUserRegistered> _onUserRegistereds;

        public UserEventService(IEnumerable<IOnUserRegistered> onUserRegistereds)
        {
            _onUserRegistereds = onUserRegistereds;
        }

        public void OnUserRegistered(User user)
        {
            _onUserRegistereds.ForEach(registered => registered.UserRegistered(user));
        }
    }
}