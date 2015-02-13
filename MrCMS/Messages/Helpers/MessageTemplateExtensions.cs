using System.Web.Mvc;
using MrCMS.Website;

namespace MrCMS.Messages.Helpers
{
    public static class MessageTemplateExtensions
    {
        public static bool IsTemplateDefault<T>(this HtmlHelper htmlHelper, T template) where T : MessageTemplate
        {
            if (template.SiteId.HasValue)
                return false;

            var provider = htmlHelper.ViewContext.HttpContext.Get<IMessageTemplateProvider>();

            var messageTemplate = provider.GetNewMessageTemplate(typeof(T));

            return template.IsHtml == messageTemplate.IsHtml &&
                   template.Bcc == messageTemplate.Bcc &&
                   template.Body == messageTemplate.Body &&
                   template.Cc == messageTemplate.Cc &&
                   template.FromAddress == messageTemplate.FromAddress &&
                   template.FromName == messageTemplate.FromName &&
                   template.Subject == messageTemplate.Subject &&
                   template.ToAddress == messageTemplate.ToAddress &&
                   template.ToName == messageTemplate.ToName;
        }
    }
}