namespace MrCMS.Web.Apps.Core.Models
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string RedirectUrl { get; set; }
        public string Message { get; set; }
    }
}