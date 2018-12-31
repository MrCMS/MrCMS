using MrCMS.Events;

namespace MrCMS.Services.Auth
{
    public interface IVerifiedPending2FA:IEvent<VerifiedPending2FAEventArgs>
    {
    }
}