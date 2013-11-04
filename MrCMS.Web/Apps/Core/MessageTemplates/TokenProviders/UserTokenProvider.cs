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
        private readonly IDocumentService _documentService;

        public UserTokenProvider(ICurrentSiteLocator currentSiteLocator, IDocumentService documentService)
        {
            _currentSiteLocator = currentSiteLocator;
            _documentService = documentService;
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
                                       var resetPasswordPage = _documentService.GetUniquePage<ResetPasswordPage>();

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