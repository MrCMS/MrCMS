namespace MrCMS.Web.Apps.Core.Models.RegisterAndLogin
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string RedirectUrl { get; set; }
        public string Message { get; set; }
    }
}