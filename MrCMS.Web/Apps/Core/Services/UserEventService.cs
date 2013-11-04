using System;
using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Core.Services
{
    public class UserEventService : IUserEventService
    {
        private readonly IEnumerable<IOnUserRegistered> _onUserRegistereds;
        private readonly IEnumerable<IOnUserLoggedIn> _onUserLoggedIns;

        public UserEventService(IEnumerable<IOnUserRegistered> onUserRegistereds, IEnumerable<IOnUserLoggedIn> onUserLoggedIns)
        {
            _onUserRegistereds = onUserRegistereds;
            _onUserLoggedIns = onUserLoggedIns;
        }

        public void OnUserRegistered(User user)
        {
            _onUserRegistereds.ForEach(registered => registered.UserRegistered(user));
        }

        public void OnUserLoggedIn(User user, Guid previousSession)
        {
            _onUserLoggedIns.ForEach(@in => @in.UserLoggedIn(user, previousSession));
        }
    }
        
    public interface IOnUserLoggedIn
    {
        void UserLoggedIn(User user, Guid previousSession);
    }
}