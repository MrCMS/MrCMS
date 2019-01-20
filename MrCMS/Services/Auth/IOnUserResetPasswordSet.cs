using MrCMS.Events;

namespace MrCMS.Services.Auth
{
    public interface IOnUserResetPasswordSet : IEvent<ResetPasswordEventArgs>
    {
    }
}