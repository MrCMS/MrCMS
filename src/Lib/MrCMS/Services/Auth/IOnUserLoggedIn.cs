using MrCMS.Events;

namespace MrCMS.Services.Auth
{
    /// <summary>
    ///     Event that passes in the user object and the guid of the anonymous user guid that the user was using before login.
    ///     Allows the copying of data from the anonymous user to the existing one
    /// </summary>
    public interface IOnUserLoggedIn : IEvent<UserLoggedInEventArgs>
    {
    }
}