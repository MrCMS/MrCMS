using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Core.MessageTemplates.TokenProviders
{
    public class ConfirmEmailTokenProvider : ITokenProvider<ConfirmEmailEmailModel>
    {
        public ConfirmEmailTokenProvider()
        {
            Tokens = GetTokens();
        }

        public IDictionary<string, Func<ConfirmEmailEmailModel, Task<string>>> Tokens { get; }

        private IDictionary<string, Func<ConfirmEmailEmailModel, Task<string>>> GetTokens()
        {
            return new Dictionary<string, Func<ConfirmEmailEmailModel, Task<string>>>
            {
                {
                    "ConfirmEmailUrl", model =>
                    {
                        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(model.Code));

                        return
                            Task.FromResult($"{model.SiteUrl}Identity/Account/ConfirmEmail?code={code}&userId={model.UserId}");
                    }
                },
            };
        }
    }
}