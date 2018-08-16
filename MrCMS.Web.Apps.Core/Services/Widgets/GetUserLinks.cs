using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Widgets;

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
                    navigationRecords.Add(new NavigationRecord
                    {
                        Text = new HtmlString(_stringResourceProvider.GetValue("My Account")),
                        Url =
                            new HtmlString(string.Format("/{0}", userAccountPage))
                    });
                }


                navigationRecords.Add(new NavigationRecord
                {
                    Text = new HtmlString(_stringResourceProvider.GetValue("Logout")),
                    Url = new HtmlString("/logout")
                });
            }
            else
            {
                var loginPageUrl = _uniquePageService.GetUrl<LoginPage>();
                if (loginPageUrl != null)
                {
                    navigationRecords.Add(new NavigationRecord
                    {
                        Text = new HtmlString(_stringResourceProvider.GetValue("Login")),
                        Url = new HtmlString(string.Format("/{0}", loginPageUrl))
                    });
                    var registerPageUrl = _uniquePageService.GetUrl<RegisterPage>();
                    if (registerPageUrl != null)
                        navigationRecords.Add(new NavigationRecord
                        {
                            Text = new HtmlString(_stringResourceProvider.GetValue("Register")),
                            Url = new HtmlString(string.Format("/{0}", registerPageUrl))
                        });
                }
            }

            return navigationRecords;
        }
    }
}