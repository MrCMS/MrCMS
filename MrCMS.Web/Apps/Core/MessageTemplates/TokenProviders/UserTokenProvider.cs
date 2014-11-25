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

        public UserTokenProvider(IUniquePageService uniquePageService)
        {
            _uniquePageService = uniquePageService;
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
                                                             ? string.Format("{0}?id={1}", resetPasswordPage.AbsoluteUrl, user.ResetPasswordGuid)
                                                             : string.Empty;

                                       return resetUrl;
                                   }
                           }
                       };
        }
    }
}