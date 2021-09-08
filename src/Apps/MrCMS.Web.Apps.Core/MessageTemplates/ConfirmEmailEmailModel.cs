namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class ConfirmEmailEmailModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string SiteUrl { get; set; }
    }
}