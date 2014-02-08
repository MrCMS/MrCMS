using System;
using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.MessageTemplates.TokenProviders
{
    public class UserTokenProvider : ITokenProvider<User>
    {
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly IUniquePageService _uniquePageService;

        public UserTokenProvider(ICurrentSiteLocator currentSiteLocator, IUniquePageService uniquePageService)
        {
            _currentSiteLocator = currentSiteLocator;
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
            var currentSite = _currentSiteLocator.GetCurrentSite();
            return new Dictionary<string, Func<User, string>>
                       {
                           {"SiteName", user => currentSite != null ? currentSite.Name : null},
                           {
                               "SiteUrl",
                               user =>
                               currentSite != null
                                   ? string.Format("http://{0}", currentSite.BaseUrl)
                                   : null
                           },
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