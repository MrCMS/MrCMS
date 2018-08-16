using System;
using System.Collections.Generic;
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
            _tokens = GetTokens();
        }

        private readonly IDictionary<string, Func<User, string>> _tokens;

        public IDictionary<string, Func<User, string>> Tokens
        {
            get { return _tokens; }
        }

        private IDictionary<string, Func<User, string>> GetTokens()
        {
            return new Dictionary<string, Func<User, string>>
                       {
                           {
                               "ResetPasswordUrl",
                               user =>
                                   {
                                       var resetPasswordPage = _uniquePageService.GetUniquePage<ResetPasswordPage>();

                                       string resetUrl = resetPasswordPage != null
                                                             ? string.Format("{0}?id={1}", _getLiveUrl.GetAbsoluteUrl(resetPasswordPage), user.ResetPasswordGuid)
                                                             : string.Empty;

                                       return resetUrl;
                                   }
                           }
                       };
        }
    }
}