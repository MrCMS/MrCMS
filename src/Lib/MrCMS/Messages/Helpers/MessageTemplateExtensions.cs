using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Website;

namespace MrCMS.Messages.Helpers
{
    public static class MessageTemplateExtensions
    {
        public static async Task<bool> IsTemplateDefault<T>(this IHtmlHelper htmlHelper, T template) where T : MessageTemplate, new()
        {
            if (template.SiteId.HasValue)
                return false;

            var provider = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IMessageTemplateProvider>();

            var messageTemplate = await provider.GetNewMessageTemplate<T>();

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