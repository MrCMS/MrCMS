using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.MessageTemplates.TokenProviders
{
    public class UserTokenProvider : ITokenProvider<User>
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly IGetLiveUrl _getLiveUrl;

        public UserTokenProvider(IUniquePageService uniquePageService, IGetLiveUrl getLiveUrl)
        {
            _uniquePageService = uniquePageService;
            _getLiveUrl = getLiveUrl;
            Tokens = GetTokens();
        }

        public IDictionary<string, Func<User, Task<string>>> Tokens { get; }

        private IDictionary<string, Func<User, Task<string>>> GetTokens()
        {
            return new Dictionary<string, Func<User, Task<string>>>
            {
                           {
                               "ResetPasswordUrl", async user =>
                                   {
                                       var resetPasswordPage = await _uniquePageService.GetUniquePage<ResetPasswordPage>();

                                       string resetUrl = resetPasswordPage != null
                                                             ? $"{await _getLiveUrl.GetAbsoluteUrl(resetPasswordPage)}?id={user.ResetPasswordGuid}"
                                                             : string.Empty;

                                       return resetUrl;
                                   }
                           }
                       };
        }
    }
}