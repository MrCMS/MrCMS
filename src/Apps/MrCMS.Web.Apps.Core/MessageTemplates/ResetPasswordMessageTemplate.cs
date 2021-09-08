using MrCMS.Messages;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class ResetPasswordMessageTemplate : MessageTemplate<ResetPasswordEmailModel>
    {
    }

    public class ResetPasswordEmailModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string SiteUrl { get; set; }
    }
}