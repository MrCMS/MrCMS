using MrCMS.Entities.People;

namespace MrCMS.Models.Auth
{
    public class Confirm2FAResult
    {
        public bool Success { get; set; }
        public User User { get; set; }
        public string Message { get; set; }
        public string ReturnUrl { get; set; }
    }
}