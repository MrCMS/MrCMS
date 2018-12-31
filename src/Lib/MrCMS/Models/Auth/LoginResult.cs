using MrCMS.Entities.People;

namespace MrCMS.Models.Auth
{
    public class LoginResult
    {
        public User User { get; set; }
        public LoginStatus Status { get; set; }
        public string ReturnUrl { get; set; }
        public string Message { get; set; }
    }

    public enum LoginStatus
    {
        Failure,
        Success,
        LockedOut,
        TwoFactorRequired
    }
}