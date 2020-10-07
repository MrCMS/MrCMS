using Microsoft.AspNetCore.Html;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Widgets;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public class GetUserLinks : GetWidgetModelBase<UserLinks>
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IUniquePageService _uniquePageService;

        public GetUserLinks(IUniquePageService uniquePageService, IStringResourceProvider stringResourceProvider,
            IGetCurrentUser getCurrentUser)
        {
            _uniquePageService = uniquePageService;
            _stringResourceProvider = stringResourceProvider;
            _getCurrentUser = getCurrentUser;
        }

        public override object GetModel(UserLinks widget)
        {
            var navigationRecords = new List<NavigationRecord>();

            var loggedIn = _getCurrentUser.Get() != null;
            if (loggedIn)
            {
                var userAccountPage = _uniquePageService.GetUrl<UserAccountPage>();
                if (!string.IsNullOrWhiteSpace(userAccountPage))
                {
                    navigationRecords.Add(new NavigationRecord(_stringResourceProvider.GetValue("My Account"),
                        $"{userAccountPage}", typeof(UserAccountPage)));
                }


                navigationRecords.Add(new NavigationRecord(_stringResourceProvider.GetValue("Logout"), "/logout"));
            }
            else
            {
                var loginPageUrl = _uniquePageService.GetUrl<LoginPage>();
                if (loginPageUrl != null)
                {
                    navigationRecords.Add(new NavigationRecord(_stringResourceProvider.GetValue("Login"), loginPageUrl,
                        typeof(LoginPage)
                    ));
                    var registerPageUrl = _uniquePageService.GetUrl<RegisterPage>();
                    if (registerPageUrl != null)
                    {
                        navigationRecords.Add(new NavigationRecord(_stringResourceProvider.GetValue("Register"),
                            registerPageUrl, typeof(RegisterPage)));
                    }
                }
            }

            return navigationRecords;
        }
    }
}