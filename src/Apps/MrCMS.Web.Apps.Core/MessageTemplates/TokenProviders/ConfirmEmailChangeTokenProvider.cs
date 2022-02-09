using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Core.MessageTemplates.TokenProviders
{
    public class ConfirmEmailChangeTokenProvider : ITokenProvider<ConfirmEmailChangeEmailModel>
    {
        public ConfirmEmailChangeTokenProvider()
        {
            Tokens = GetTokens();
        }

        public IDictionary<string, Func<ConfirmEmailChangeEmailModel, Task<string>>> Tokens { get; }

        private IDictionary<string, Func<ConfirmEmailChangeEmailModel, Task<string>>> GetTokens()
        {
            return new Dictionary<string, Func<ConfirmEmailChangeEmailModel, Task<string>>>
            {
                {
                    "ConfirmEmailChangeUrl",
                    model =>
                    {
                        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(model.Code));

                        return
                            Task.FromResult(
                                $"{model.SiteUrl}Identity/Account/ConfirmEmailChange?code={code}&email={model.NewEmail}&userId={model.UserId}");
                    }
                },
            };
        }
    }
}