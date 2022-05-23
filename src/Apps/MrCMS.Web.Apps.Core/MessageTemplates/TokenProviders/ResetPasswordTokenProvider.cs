using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Core.MessageTemplates.TokenProviders
{
    public class ResetPasswordTokenProvider : ITokenProvider<ResetPasswordEmailModel>
    {
        public ResetPasswordTokenProvider()
        {
            _tokens = GetTokens();
        }

        private readonly IDictionary<string, Func<ResetPasswordEmailModel, Task<string>>> _tokens;

        public IDictionary<string, Func<ResetPasswordEmailModel, Task<string>>> Tokens
        {
            get { return _tokens; }
        }

        private IDictionary<string, Func<ResetPasswordEmailModel, Task<string>>> GetTokens()
        {
            return new Dictionary<string, Func<ResetPasswordEmailModel, Task<string>>>
            {
                {
                    "ResetPasswordUrl",
                    model =>
                    {
                        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(model.Key));

                        return Task.FromResult($"{model.SiteUrl}Identity/Account/ResetPassword?code={code}");
                    }
                },
            };
        }
    }
}