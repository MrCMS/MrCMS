namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class ConfirmEmailChangeEmailModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string NewEmail { get; set; }
        public string SiteUrl { get; set; }
    }
}