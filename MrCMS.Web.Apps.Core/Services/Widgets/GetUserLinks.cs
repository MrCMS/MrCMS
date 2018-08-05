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
                var userAccountPage = _uniquePageService.GetUniquePage<UserAccountPage>();
                if (userAccountPage != null)
                {
                    var liveUrlSegment = userAccountPage.LiveUrlSegment;
                    navigationRecords.Add(new NavigationRecord
                    {
                        Text = new HtmlString(_stringResourceProvider.GetValue("My Account")),
                        Url =
                            new HtmlString(string.Format("/{0}", liveUrlSegment))
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
                var liveUrlSegment = _uniquePageService.GetUniquePage<LoginPage>().LiveUrlSegment;
                if (liveUrlSegment != null)
                {
                    navigationRecords.Add(new NavigationRecord
                    {
                        Text = new HtmlString(_stringResourceProvider.GetValue("Login")),
                        Url = new HtmlString(string.Format("/{0}", liveUrlSegment))
                    });
                    var urlSegment = _uniquePageService.GetUniquePage<RegisterPage>().LiveUrlSegment;
                    if (urlSegment != null)
                        navigationRecords.Add(new NavigationRecord
                        {
                            Text = new HtmlString(_stringResourceProvider.GetValue("Register")),
                            Url = new HtmlString(string.Format("/{0}", urlSegment))
                        });
                }
            }

            return navigationRecords;
        }
    }
}